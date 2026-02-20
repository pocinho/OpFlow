// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow;

/// <summary>
/// Provides helper methods for inspecting <see cref="Operation{T}"/> instances
/// without altering their semantics. These helpers are intentionally non-
/// transformational and serve as ergonomic utilities for imperative code paths.
/// </summary>
public static class OperationExtensions
{
    // ------------------------------------------------------------
    // 1. Basic Helpers
    // ------------------------------------------------------------

    /// <summary>
    /// Determines whether the operation represents a successful result.
    /// </summary>
    public static bool IsSuccess<T>(this Operation<T> op)
        => op is Operation<T>.Success;

    /// <summary>
    /// Determines whether the operation represents a failure.
    /// </summary>
    public static bool IsFailure<T>(this Operation<T> op)
        => op is Operation<T>.Failure;

    /// <summary>
    /// Attempts to extract the successful result value.
    /// </summary>
    /// <param name="op">The operation to inspect.</param>
    /// <param name="value">The extracted value if the operation succeeded.</param>
    /// <returns><c>true</c> if the operation succeeded; otherwise <c>false</c>.</returns>
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

    /// <summary>
    /// Attempts to extract the error from a failed operation.
    /// </summary>
    /// <param name="op">The operation to inspect.</param>
    /// <param name="error">The extracted error if the operation failed.</param>
    /// <returns><c>true</c> if the operation failed; otherwise <c>false</c>.</returns>
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

    // ------------------------------------------------------------
    // 2. Async Helpers (Symmetric)
    // ------------------------------------------------------------

    /// <summary>
    /// Awaits the operation task and attempts to extract the successful result.
    /// </summary>
    /// <param name="opTask">A task producing an operation.</param>
    /// <returns>
    /// A tuple containing a boolean indicating success and the extracted value.
    /// </returns>
    public static async Task<(bool ok, T value)> TryGetAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op is Operation<T>.Success s
            ? (true, s.Result)
            : (false, default!);
    }

    /// <summary>
    /// Awaits the operation task and attempts to extract the error.
    /// </summary>
    /// <param name="opTask">A task producing an operation.</param>
    /// <returns>
    /// A tuple containing a boolean indicating failure and the extracted error.
    /// </returns>
    public static async Task<(bool ok, Error error)> TryGetErrorAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);

        return op is Operation<T>.Failure f
            ? (true, f.Error)
            : (false, default!);
    }

    /// <summary>
    /// Awaits the operation task and returns <c>true</c> if it represents success.
    /// </summary>
    public static async Task<bool> IsSuccessAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);
        return op is Operation<T>.Success;
    }

    /// <summary>
    /// Awaits the operation task and returns <c>true</c> if it represents failure.
    /// </summary>
    public static async Task<bool> IsFailureAsync<T>(this Task<Operation<T>> opTask)
    {
        Operation<T> op = await opTask.ConfigureAwait(false);
        return op is Operation<T>.Failure;
    }
}