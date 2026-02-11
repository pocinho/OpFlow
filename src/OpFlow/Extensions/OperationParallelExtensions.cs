// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Extensions;

public static class OperationParallelExtensions
{
    // ------------------------------------------------------------
    // 1. SYNC PARALLEL COMPOSITION
    // ------------------------------------------------------------

    public static Operation<(T1, T2)> WhenAll<T1, T2>(
        Operation<T1> op1,
        Operation<T2> op2)
    {
        if (op1 is Operation<T1>.Failure f1)
            return new Operation<(T1, T2)>.Failure(f1.Error);

        if (op2 is Operation<T2>.Failure f2)
            return new Operation<(T1, T2)>.Failure(f2.Error);

        return new Operation<(T1, T2)>.Success(
            (Get(op1), Get(op2))
        );
    }

    public static Operation<(T1, T2, T3)> WhenAll<T1, T2, T3>(
        Operation<T1> op1,
        Operation<T2> op2,
        Operation<T3> op3)
    {
        if (op1 is Operation<T1>.Failure f1)
            return new Operation<(T1, T2, T3)>.Failure(f1.Error);

        if (op2 is Operation<T2>.Failure f2)
            return new Operation<(T1, T2, T3)>.Failure(f2.Error);

        if (op3 is Operation<T3>.Failure f3)
            return new Operation<(T1, T2, T3)>.Failure(f3.Error);

        return new Operation<(T1, T2, T3)>.Success(
            (Get(op1), Get(op2), Get(op3))
        );
    }

    public static Operation<IReadOnlyList<T>> WhenAll<T>(
        params Operation<T>[] operations)
    {
        foreach (Operation<T> op in operations)
        {
            if (op is Operation<T>.Failure f)
                return new Operation<IReadOnlyList<T>>.Failure(f.Error);
        }

        return new Operation<IReadOnlyList<T>>.Success(
            operations.Select(Get).ToList()
        );
    }

    // ------------------------------------------------------------
    // 2. ASYNC PARALLEL COMPOSITION
    // ------------------------------------------------------------

    public static async Task<Operation<(T1, T2)>> WhenAllAsync<T1, T2>(
        Task<Operation<T1>> op1Task,
        Task<Operation<T2>> op2Task)
    {
        Operation<T1> op1 = await op1Task.ConfigureAwait(false);
        Operation<T2> op2 = await op2Task.ConfigureAwait(false);

        if (op1 is Operation<T1>.Failure f1)
            return new Operation<(T1, T2)>.Failure(f1.Error);

        if (op2 is Operation<T2>.Failure f2)
            return new Operation<(T1, T2)>.Failure(f2.Error);

        return new Operation<(T1, T2)>.Success(
            (Get(op1), Get(op2))
        );
    }

    public static async Task<Operation<(T1, T2, T3)>> WhenAllAsync<T1, T2, T3>(
        Task<Operation<T1>> op1Task,
        Task<Operation<T2>> op2Task,
        Task<Operation<T3>> op3Task)
    {
        Operation<T1> op1 = await op1Task.ConfigureAwait(false);
        Operation<T2> op2 = await op2Task.ConfigureAwait(false);
        Operation<T3> op3 = await op3Task.ConfigureAwait(false);

        if (op1 is Operation<T1>.Failure f1)
            return new Operation<(T1, T2, T3)>.Failure(f1.Error);

        if (op2 is Operation<T2>.Failure f2)
            return new Operation<(T1, T2, T3)>.Failure(f2.Error);

        if (op3 is Operation<T3>.Failure f3)
            return new Operation<(T1, T2, T3)>.Failure(f3.Error);

        return new Operation<(T1, T2, T3)>.Success(
            (Get(op1), Get(op2), Get(op3))
        );
    }

    public static async Task<Operation<IReadOnlyList<T>>> WhenAllAsync<T>(
        params Task<Operation<T>>[] tasks)
    {
        Operation<T>[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

        foreach (Operation<T> op in results)
        {
            if (op is Operation<T>.Failure f)
                return new Operation<IReadOnlyList<T>>.Failure(f.Error);
        }

        return new Operation<IReadOnlyList<T>>.Success(
            results.Select(Get).ToList()
        );
    }

    // ------------------------------------------------------------
    // 3. INTERNAL HELPER
    // ------------------------------------------------------------

    private static T Get<T>(Operation<T> op)
        => op is Operation<T>.Success s
            ? s.Result
            : throw new InvalidOperationException("Cannot extract value from Failure.");
}