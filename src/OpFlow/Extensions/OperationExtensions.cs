// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Extensions;

public static class OperationExtensions
{
    // ------------------------------------------------------------
    // 1. Basic Helpers
    // ------------------------------------------------------------

    public static bool IsSuccess<T>(this Operation<T> op)
        => op is Operation<T>.Success;

    public static bool IsFailure<T>(this Operation<T> op)
        => op is Operation<T>.Failure;

    public static bool TryGet<T>(this Operation<T> op, out T value)
    {
        if (op is Operation<T>.Success s)
        {
            value = s.Result;
            return true;
        }

        value = default!;
        return false;
    }

    public static bool TryGetError<T>(this Operation<T> op, out Error error)
    {
        if (op is Operation<T>.Failure f)
        {
            error = f.Error;
            return true;
        }

        error = default!;
        return false;
    }

    // Async versions
    public static async Task<(bool ok, T value)> TryGetAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);
        return op is Operation<T>.Success s
            ? (true, s.Result)
            : (false, default!);
    }

    public static async Task<(bool ok, Error error)> TryGetErrorAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);
        return op is Operation<T>.Failure f
            ? (true, f.Error)
            : (false, default!);
    }

    // ------------------------------------------------------------
    // 2. Tap (side-effects without changing the value)
    // ------------------------------------------------------------

    public static Operation<T> Tap<T>(this Operation<T> op, Action<T> action)
    {
        if (op is Operation<T>.Success s)
            action(s.Result);

        return op;
    }

    public static Operation<T> TapError<T>(this Operation<T> op, Action<Error> action)
    {
        if (op is Operation<T>.Failure f)
            action(f.Error);

        return op;
    }

    // Async versions
    public static async Task<Operation<T>> TapAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task> actionAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        if (op is Operation<T>.Success s)
            await actionAsync(s.Result).ConfigureAwait(false);

        return op;
    }

    public static async Task<Operation<T>> TapErrorAsync<T>(
        this Task<Operation<T>> opTask,
        Func<Error, Task> actionAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        if (op is Operation<T>.Failure f)
            await actionAsync(f.Error).ConfigureAwait(false);

        return op;
    }

    // ------------------------------------------------------------
    // 3. OnSuccess / OnFailure (fluent branching)
    // ------------------------------------------------------------

    public static Operation<T> OnSuccess<T>(this Operation<T> op, Action<T> action)
    {
        if (op is Operation<T>.Success s)
            action(s.Result);

        return op;
    }

    public static Operation<T> OnFailure<T>(this Operation<T> op, Action<Error> action)
    {
        if (op is Operation<T>.Failure f)
            action(f.Error);

        return op;
    }

    // Async versions
    public static async Task<Operation<T>> OnSuccessAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task> actionAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        if (op is Operation<T>.Success s)
            await actionAsync(s.Result).ConfigureAwait(false);

        return op;
    }

    public static async Task<Operation<T>> OnFailureAsync<T>(
        this Task<Operation<T>> opTask,
        Func<Error, Task> actionAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        if (op is Operation<T>.Failure f)
            await actionAsync(f.Error).ConfigureAwait(false);

        return op;
    }

    // ------------------------------------------------------------
    // 4. Map (transform success)
    // ------------------------------------------------------------

    public static Operation<TResult> Map<T, TResult>(
        this Operation<T> op,
        Func<T, TResult> map)
    {
        return op switch
        {
            Operation<T>.Success s => new Operation<TResult>.Success(map(s.Result)),
            Operation<T>.Failure f => new Operation<TResult>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // Async versions
    public static async Task<Operation<TResult>> MapAsync<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<TResult>> mapAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s => new Operation<TResult>.Success(await mapAsync(s.Result)),
            Operation<T>.Failure f => new Operation<TResult>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 5. Bind (monadic composition)
    // ------------------------------------------------------------

    public static Operation<TResult> Bind<T, TResult>(
        this Operation<T> op,
        Func<T, Operation<TResult>> bind)
    {
        return op switch
        {
            Operation<T>.Success s => bind(s.Result),
            Operation<T>.Failure f => new Operation<TResult>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // Async versions
    public static async Task<Operation<TResult>> BindAsync<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<Operation<TResult>>> bindAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s => await bindAsync(s.Result),
            Operation<T>.Failure f => new Operation<TResult>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 6. Recover (fallback on failure)
    // ------------------------------------------------------------

    public static Operation<T> Recover<T>(
        this Operation<T> op,
        Func<Error, T> fallback)
    {
        return op switch
        {
            Operation<T>.Success => op,
            Operation<T>.Failure f => new Operation<T>.Success(fallback(f.Error)),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // Async versions
    public static async Task<Operation<T>> RecoverAsync<T>(
        this Task<Operation<T>> opTask,
        Func<Error, Task<T>> fallbackAsync)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success => op,
            Operation<T>.Failure f => new Operation<T>.Success(await fallbackAsync(f.Error)),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 7. Match (functional branching)
    // ------------------------------------------------------------

    public static TResult Match<T, TResult>(
        this Operation<T> op,
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return op switch
        {
            Operation<T>.Success s => onSuccess(s.Result),
            Operation<T>.Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    public static void Match<T>(
        this Operation<T> op,
        Action<T> onSuccess,
        Action<Error> onFailure)
    {
        if (onSuccess is null)
            throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure is null)
            throw new ArgumentNullException(nameof(onFailure));

        switch (op)
        {
            case Operation<T>.Success s:
                onSuccess(s.Result);
                break;

            case Operation<T>.Failure f:
                onFailure(f.Error);
                break;

            default:
                throw new InvalidOperationException("Unknown Operation state.");
        }
    }

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s => await onSuccess(s.Result),
            Operation<T>.Failure f => await onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    public static async Task MatchAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task> onSuccess,
        Func<Error, Task> onFailure)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        switch (op)
        {
            case Operation<T>.Success s:
                await onSuccess(s.Result);
                break;

            case Operation<T>.Failure f:
                await onFailure(f.Error);
                break;

            default:
                throw new InvalidOperationException("Unknown Operation state.");
        }
    }
}