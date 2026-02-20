// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow;

/// <summary>
/// The Op facade is the recommended entry point for creating and validating
/// <see cref="Operation{T}"/> values. It intentionally exposes only:
/// 
/// • Creation helpers (Success, Failure, From, Try)
/// • Boundary helpers (FromAsync, FromException)
/// • Validation helpers (Ensure, Require, Validate, ValidateAll)
/// 
/// All core monadic operators (Map, Bind, Tap, Recover, Match, etc.)
/// are defined exclusively on <see cref="Operation"/> to avoid duplication
/// and ensure a single canonical implementation.
/// </summary>
public static class Op
{
    // ------------------------------------------------------------
    // 0. Creation / Boundary
    // ------------------------------------------------------------

    /// <summary>
    /// Creates a successful operation containing the specified value.
    /// </summary>
    /// <example>
    /// var op = Op.Success(42);
    /// </example>
    public static Operation<T> Success<T>(T value)
        => Operation.Success(value);

    /// <summary>
    /// Creates a failed operation with the specified error.
    /// </summary>
    /// <example>
    /// var op = Op.Failure&lt;int&gt;(new Error.Validation("Invalid"));
    /// </example>
    public static Operation<T> Failure<T>(Error error)
        => Operation.FailureOf<T>(error);

    /// <summary>
    /// Wraps a raw value into a successful operation.
    /// </summary>
    /// <example>
    /// var op = Op.From("hello");
    /// </example>
    public static Operation<T> From<T>(T value)
        => Operation.FromValue(value);

    /// <summary>
    /// Executes a function and wraps its result in an operation.
    /// Exceptions are captured as <see cref="Error.Unexpected"/>.
    /// </summary>
    /// <example>
    /// var op = Op.From(() => File.ReadAllText("config.json"));
    /// </example>
    public static Operation<T> From<T>(Func<T> func)
        => Operation.From(func);

    /// <summary>
    /// Executes an asynchronous function and wraps its result in an operation.
    /// Exceptions are captured as <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Task<Operation<T>> FromAsync<T>(Func<Task<T>> func)
        => Operation.FromAsync(func);

    /// <summary>
    /// Wraps an existing <see cref="Task{T}"/> into an operation.
    /// Exceptions are captured as <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Task<Operation<T>> FromAsync<T>(Task<T> task)
        => Operation.FromAsync(task);

    ///// <summary>
    ///// Creates a failed operation from an error.
    ///// </summary>
    //public static Operation<T> FromError<T>(Error error)
    //    => Operation.FromError<T>(error);

    /// <summary>
    /// Converts an exception into a failed operation.
    /// </summary>
    public static Operation<T> FromException<T>(Exception ex)
        => Operation.FromException<T>(ex);

    /// <summary>
    /// Executes a function and captures exceptions as <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Operation<T> Try<T>(Func<T> func)
        => Operation.Try(func);

    /// <summary>
    /// Executes an asynchronous function and captures exceptions as <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Task<Operation<T>> TryAsync<T>(Func<Task<T>> func)
        => Operation.TryAsync(func);

    /// <summary>
    /// Wraps an existing task and captures exceptions as <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Task<Operation<T>> TryAsync<T>(Task<T> task)
        => Operation.TryAsync(task);
}