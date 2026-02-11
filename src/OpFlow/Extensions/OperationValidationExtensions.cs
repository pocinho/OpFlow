// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow;

public static class OperationValidationExtensions
{
    // ------------------------------------------------------------
    // 1. Ensure (sync)
    // ------------------------------------------------------------

    public static Operation<T> Ensure<T>(
        this Operation<T> op,
        Func<T, bool> predicate,
        Func<T, Error> errorFactory)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));
        if (errorFactory is null)
            throw new ArgumentNullException(nameof(errorFactory));

        return op switch
        {
            Operation<T>.Success s =>
                predicate(s.Result)
                    ? op
                    : new Operation<T>.Failure(errorFactory(s.Result)),

            Operation<T>.Failure =>
                op,

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }


    // ------------------------------------------------------------
    // 2. EnsureAsync (async predicate)
    // ------------------------------------------------------------

    public static async Task<Operation<T>> EnsureAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task<bool>> predicateAsync,
        Func<T, Error> errorFactory)
    {
        if (predicateAsync is null)
            throw new ArgumentNullException(nameof(predicateAsync));
        if (errorFactory is null)
            throw new ArgumentNullException(nameof(errorFactory));

        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s =>
                await predicateAsync(s.Result).ConfigureAwait(false)
                    ? op
                    : new Operation<T>.Failure(errorFactory(s.Result)),

            Operation<T>.Failure =>
                op,

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }


    // ------------------------------------------------------------
    // 3. Require (sync)
    // ------------------------------------------------------------

    public static Operation<T> Require<T>(
        this Operation<T> op,
        Func<T, bool> predicate,
        Error error)
        => op.Ensure(predicate, _ => error);


    // ------------------------------------------------------------
    // 4. RequireAsync (async)
    // ------------------------------------------------------------

    public static Task<Operation<T>> RequireAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task<bool>> predicateAsync,
        Error error)
        => opTask.EnsureAsync(predicateAsync, _ => error);


    // ------------------------------------------------------------
    // 5. Validate (short-circuiting)
    // ------------------------------------------------------------

    public static Operation<T> Validate<T>(
        this Operation<T> op,
        params Func<T, Operation<T>>[] validators)
    {
        if (validators is null)
            throw new ArgumentNullException(nameof(validators));

        foreach (Func<T, Operation<T>> validate in validators)
        {
            if (op is Operation<T>.Failure)
                return op;

            op = validate(((Operation<T>.Success)op).Result);
        }

        return op;
    }


    // ------------------------------------------------------------
    // 6. ValidateAsync (short-circuiting async)
    // ------------------------------------------------------------

    public static async Task<Operation<T>> ValidateAsync<T>(
        this Task<Operation<T>> opTask,
        params Func<T, Task<Operation<T>>>[] validators)
    {
        if (validators is null)
            throw new ArgumentNullException(nameof(validators));

        Operation<T> op = await opTask.ConfigureAwait(false);

        foreach (Func<T, Task<Operation<T>>> validateAsync in validators)
        {
            if (op is Operation<T>.Failure)
                return op;

            op = await validateAsync(((Operation<T>.Success)op).Result)
                .ConfigureAwait(false);
        }

        return op;
    }


    // ------------------------------------------------------------
    // 7. ValidateAll (accumulate errors)
    // ------------------------------------------------------------

    public static Operation<T> ValidateAll<T>(
        this Operation<T> op,
        params Func<T, Operation<T>>[] validators)
    {
        if (validators is null)
            throw new ArgumentNullException(nameof(validators));

        if (op is Operation<T>.Failure f)
            return f;

        T value = ((Operation<T>.Success)op).Result;
        List<Error> errors = new List<Error>();

        foreach (Func<T, Operation<T>> validate in validators)
        {
            Operation<T> result = validate(value);
            if (result is Operation<T>.Failure vf)
                errors.Add(vf.Error);
        }

        return errors.Count switch
        {
            0 => op,
            1 => new Operation<T>.Failure(errors[0]),
            _ => new Operation<T>.Failure(
                    new Error.Validation(
                        "Multiple validation errors",
                        errors.Select(e => e.ToString()).ToList()
                    )
                 )
        };
    }


    // ------------------------------------------------------------
    // 8. ValidateAllAsync (accumulate async errors)
    // ------------------------------------------------------------

    public static async Task<Operation<T>> ValidateAllAsync<T>(
        this Task<Operation<T>> opTask,
        params Func<T, Task<Operation<T>>>[] validators)
    {
        if (validators is null)
            throw new ArgumentNullException(nameof(validators));

        Operation<T> op = await opTask.ConfigureAwait(false);

        if (op is Operation<T>.Failure f)
            return f;

        T value = ((Operation<T>.Success)op).Result;
        List<Error> errors = new List<Error>();

        foreach (Func<T, Task<Operation<T>>> validateAsync in validators)
        {
            Operation<T> result = await validateAsync(value).ConfigureAwait(false);
            if (result is Operation<T>.Failure vf)
                errors.Add(vf.Error);
        }

        return errors.Count switch
        {
            0 => op,
            1 => new Operation<T>.Failure(errors[0]),
            _ => new Operation<T>.Failure(
                    new Error.Validation(
                        "Multiple validation errors",
                        errors.Select(e => e.ToString()).ToList()
                    )
                 )
        };
    }


    // ------------------------------------------------------------
    // 9. Common validation helpers
    // ------------------------------------------------------------

    public static Operation<T> NotNull<T>(this Operation<T?> op, string? fieldName = null)
    {
        return op.Ensure(
            v => v is not null,
            _ => new Error.Validation($"{fieldName ?? "Value"} must not be null")
        )!;
    }

    public static Operation<string> NotEmpty(this Operation<string> op, string? fieldName = null)
    {
        return op.Ensure(
            v => !string.IsNullOrWhiteSpace(v),
            _ => new Error.Validation($"{fieldName ?? "Value"} must not be empty")
        );
    }
}