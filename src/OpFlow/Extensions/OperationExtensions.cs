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
}