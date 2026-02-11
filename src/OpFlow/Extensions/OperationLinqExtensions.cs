// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Extensions;

public static class OperationLinqExtensions
{
    // ------------------------------------------------------------
    // 1. Select (Map)
    // ------------------------------------------------------------
    public static Operation<TResult> Select<T, TResult>(
        this Operation<T> op,
        Func<T, TResult> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        return op switch
        {
            Operation<T>.Success s => new Operation<TResult>.Success(selector(s.Result)),
            Operation<T>.Failure f => new Operation<TResult>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 1. SelectAsync (MapAsync)
    // ------------------------------------------------------------
    public static async Task<Operation<TResult>> SelectAsync<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<TResult>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s =>
                new Operation<TResult>.Success(await selector(s.Result).ConfigureAwait(false)),

            Operation<T>.Failure f =>
                new Operation<TResult>.Failure(f.Error),

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 2. SelectMany (Bind)
    // ------------------------------------------------------------
    public static Operation<TResult> SelectMany<T, TIntermediate, TResult>(
        this Operation<T> op,
        Func<T, Operation<TIntermediate>> bind,
        Func<T, TIntermediate, TResult> project)
    {
        if (bind is null)
            throw new ArgumentNullException(nameof(bind));
        if (project is null)
            throw new ArgumentNullException(nameof(project));

        return op switch
        {
            Operation<T>.Success s =>
                bind(s.Result) switch
                {
                    Operation<TIntermediate>.Success si =>
                        new Operation<TResult>.Success(project(s.Result, si.Result)),

                    Operation<TIntermediate>.Failure fi =>
                        new Operation<TResult>.Failure(fi.Error),

                    _ => throw new InvalidOperationException("Unknown Operation state.")
                },

            Operation<T>.Failure f =>
                new Operation<TResult>.Failure(f.Error),

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 2. SelectManyAsync (BindAsync)
    // ------------------------------------------------------------
    public static async Task<Operation<TResult>> SelectManyAsync<T, TIntermediate, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<Operation<TIntermediate>>> bindAsync,
        Func<T, TIntermediate, TResult> project)
    {
        if (bindAsync is null)
            throw new ArgumentNullException(nameof(bindAsync));
        if (project is null)
            throw new ArgumentNullException(nameof(project));

        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s =>
                await bindAsync(s.Result).ConfigureAwait(false) switch
                {
                    Operation<TIntermediate>.Success si =>
                        new Operation<TResult>.Success(project(s.Result, si.Result)),

                    Operation<TIntermediate>.Failure fi =>
                        new Operation<TResult>.Failure(fi.Error),

                    _ => throw new InvalidOperationException("Unknown Operation state.")
                },

            Operation<T>.Failure f =>
                new Operation<TResult>.Failure(f.Error),

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 3. Where (filtering)
    // ------------------------------------------------------------
    public static Operation<T> Where<T>(
        this Operation<T> op,
        Func<T, bool> predicate)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        return op switch
        {
            Operation<T>.Success s =>
                predicate(s.Result)
                    ? op
                    : new Operation<T>.Failure(new Error.Validation("Predicate failed")),

            Operation<T>.Failure =>
                op,

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

    // ------------------------------------------------------------
    // 3. WhereAsync (async predicate)
    // ------------------------------------------------------------
    public static async Task<Operation<T>> WhereAsync<T>(
        this Task<Operation<T>> opTask,
        Func<T, Task<bool>> predicateAsync)
    {
        if (predicateAsync is null)
            throw new ArgumentNullException(nameof(predicateAsync));

        Operation<T> op = await opTask.ConfigureAwait(false);

        return op switch
        {
            Operation<T>.Success s =>
                await predicateAsync(s.Result).ConfigureAwait(false)
                    ? op
                    : new Operation<T>.Failure(new Error.Validation("Predicate failed")),

            Operation<T>.Failure =>
                op,

            _ => throw new InvalidOperationException("Unknown Operation state.")
        };
    }

}