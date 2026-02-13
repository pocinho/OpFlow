// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow;

using OpFlow.Extensions;

public static class Op
{
    // ------------------------------------------------------------
    // 0. Constructors
    // ------------------------------------------------------------

    public static Operation<T> Success<T>(T value)
        => new Operation<T>.Success(value);

    public static Operation<T> Failure<T>(Error error)
        => new Operation<T>.Failure(error);

    public static Operation<T> From<T>(T value)
        => new Operation<T>.Success(value);


    // ------------------------------------------------------------
    // 1. Parallel Composition
    // ------------------------------------------------------------

    public static Operation<(T1, T2)> WhenAll<T1, T2>(
        Operation<T1> op1,
        Operation<T2> op2)
        => OperationParallelExtensions.WhenAll(op1, op2);

    public static Operation<(T1, T2, T3)> WhenAll<T1, T2, T3>(
        Operation<T1> op1,
        Operation<T2> op2,
        Operation<T3> op3)
        => OperationParallelExtensions.WhenAll(op1, op2, op3);

    public static Operation<IReadOnlyList<T>> WhenAll<T>(
        params Operation<T>[] ops)
        => OperationParallelExtensions.WhenAll(ops);

    public static Task<Operation<(T1, T2)>> WhenAllAsync<T1, T2>(
        Task<Operation<T1>> op1,
        Task<Operation<T2>> op2)
        => OperationParallelExtensions.WhenAllAsync(op1, op2);

    public static Task<Operation<(T1, T2, T3)>> WhenAllAsync<T1, T2, T3>(
        Task<Operation<T1>> op1,
        Task<Operation<T2>> op2,
        Task<Operation<T3>> op3)
        => OperationParallelExtensions.WhenAllAsync(op1, op2, op3);

    public static Task<Operation<IReadOnlyList<T>>> WhenAllAsync<T>(
        params Task<Operation<T>>[] tasks)
        => OperationParallelExtensions.WhenAllAsync(tasks);


    // ------------------------------------------------------------
    // 2. Validation
    // ------------------------------------------------------------

    public static Operation<T> Ensure<T>(
        Operation<T> op,
        Func<T, bool> predicate,
        Func<T, Error> errorFactory)
        => op.Ensure(predicate, errorFactory);

    public static Task<Operation<T>> EnsureAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task<bool>> predicateAsync,
        Func<T, Error> errorFactory)
        => op.EnsureAsync(predicateAsync, errorFactory);

    public static Operation<T> Require<T>(
        Operation<T> op,
        Func<T, bool> predicate,
        Error error)
        => op.Require(predicate, error);

    public static Task<Operation<T>> RequireAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task<bool>> predicateAsync,
        Error error)
        => op.RequireAsync(predicateAsync, error);

    public static Operation<T> Validate<T>(
        Operation<T> op,
        params Func<T, Error?>[] rules)
        => op.Validate(rules);

    public static Task<Operation<T>> ValidateAsync<T>(
        Task<Operation<T>> op,
        params Func<T, Task<Error?>>[] rules)
        => op.ValidateAsync(rules);

    public static Operation<T> ValidateAll<T>(
        Operation<T> op,
        params Func<T, Error?>[] rules)
        => op.ValidateAll(rules);

    public static Task<Operation<T>> ValidateAllAsync<T>(
        Task<Operation<T>> op,
        params Func<T, Task<Error?>>[] rules)
        => op.ValidateAllAsync(rules);


    // ------------------------------------------------------------
    // 3. LINQ / Monad
    // ------------------------------------------------------------

    public static Operation<TResult> Select<T, TResult>(
        Operation<T> op,
        Func<T, TResult> selector)
        => op.Select(selector);

    public static Task<Operation<TResult>> SelectAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> selector)
        => op.SelectAsync(selector);

    public static Operation<TResult> SelectMany<T, TIntermediate, TResult>(
        Operation<T> op,
        Func<T, Operation<TIntermediate>> bind,
        Func<T, TIntermediate, TResult> project)
        => op.SelectMany(bind, project);

    public static Task<Operation<TResult>> SelectManyAsync<T, TIntermediate, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<Operation<TIntermediate>>> bindAsync,
        Func<T, TIntermediate, TResult> project)
        => op.SelectManyAsync(bindAsync, project);

    public static Task<Operation<TResult>> SelectMany<T, TIntermediate, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<Operation<TIntermediate>>> bindAsync,
        Func<T, TIntermediate, TResult> project)
        => SelectManyAsync(opTask, bindAsync, project);

    public static Task<Operation<TResult>> Select<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, TResult> selector)
        => opTask.SelectAsync(x => Task.FromResult(selector(x)));

    // ------------------------------------------------------------
    // 4. Map / Bind
    // ------------------------------------------------------------

    public static Operation<TResult> Map<T, TResult>(
        Operation<T> op,
        Func<T, TResult> map)
        => op.Map(map);

    public static Task<Operation<TResult>> MapAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> mapAsync)
        => op.MapAsync(mapAsync);

    public static Operation<TResult> Bind<T, TResult>(
        Operation<T> op,
        Func<T, Operation<TResult>> bind)
        => op.Bind(bind);

    public static Task<Operation<TResult>> BindAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<Operation<TResult>>> bindAsync)
        => op.BindAsync(bindAsync);


    // ------------------------------------------------------------
    // 5. Tap / OnSuccess / OnFailure
    // ------------------------------------------------------------

    public static Operation<T> Tap<T>(
        Operation<T> op,
        Action<T> action)
        => op.Tap(action);

    public static Operation<T> TapError<T>(
        Operation<T> op,
        Action<Error> action)
        => op.TapError(action);

    public static Task<Operation<T>> TapAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task> actionAsync)
        => op.TapAsync(actionAsync);

    public static Task<Operation<T>> TapErrorAsync<T>(
        Task<Operation<T>> op,
        Func<Error, Task> actionAsync)
        => op.TapErrorAsync(actionAsync);

    public static Operation<T> OnSuccess<T>(
        Operation<T> op,
        Action<T> action)
        => op.OnSuccess(action);

    public static Operation<T> OnFailure<T>(
        Operation<T> op,
        Action<Error> action)
        => op.OnFailure(action);


    // ------------------------------------------------------------
    // 6. Recover
    // ------------------------------------------------------------

    public static Operation<T> Recover<T>(
        Operation<T> op,
        Func<Error, T> fallback)
        => op.Recover(fallback);

    public static Task<Operation<T>> RecoverAsync<T>(
        Task<Operation<T>> op,
        Func<Error, Task<T>> fallbackAsync)
        => op.RecoverAsync(fallbackAsync);


    // ------------------------------------------------------------
    // 7. Match
    // ------------------------------------------------------------

    public static TResult Match<T, TResult>(
        Operation<T> op,
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
        => op.Match(onSuccess, onFailure);

    public static void Match<T>(
        Operation<T> op,
        Action<T> onSuccess,
        Action<Error> onFailure)
        => op.Match(onSuccess, onFailure);

    public static Task<TResult> MatchAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure)
        => op.MatchAsync(onSuccess, onFailure);

    public static Task MatchAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task> onSuccess,
        Func<Error, Task> onFailure)
        => op.MatchAsync(onSuccess, onFailure);
}