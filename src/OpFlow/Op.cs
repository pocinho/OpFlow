// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow;

/// <summary>
/// The Op facade is the recommended entry point for creating and validating
/// <see cref="Operation{T}"/> values. It intentionally exposes only:
/// 
/// • Creation helpers (Success, Failure, From, Try)
/// • Boundary helpers (FromAsync, FromException, FromNullable)
/// • Validation helpers (Ensure, Require, Validate, ValidateAll)
/// • Parallel composition (WhenAll)
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

    /// <summary>
    /// Creates a failed operation from an error.
    /// </summary>
    public static Operation<T> FromError<T>(Error error)
        => Operation.FromError<T>(error);

    /// <summary>
    /// Converts an exception into a failed operation.
    /// </summary>
    public static Operation<T> FromException<T>(Exception ex)
        => Operation.FromException<T>(ex);

    /// <summary>
    /// Wraps a nullable reference into an operation.
    /// If the value is null, returns a failure with <see cref="Error.Unexpected"/>.
    /// </summary>
    public static Operation<T> FromNullable<T>(T? value, string? message = null)
        where T : class
        => Operation.FromNullable(value, message);

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


    // ------------------------------------------------------------
    // 1. Parallel Composition
    // ------------------------------------------------------------

    /// <summary>
    /// Combines two operations into a tuple.
    /// If either fails, the first failure is returned.
    /// </summary>
    public static Operation<(T1, T2)> WhenAll<T1, T2>(
        Operation<T1> op1,
        Operation<T2> op2)
        => OperationParallelExtensions.WhenAll(op1, op2);

    /// <summary>
    /// Combines three operations into a tuple.
    /// If any fail, the first failure is returned.
    /// </summary>
    public static Operation<(T1, T2, T3)> WhenAll<T1, T2, T3>(
        Operation<T1> op1,
        Operation<T2> op2,
        Operation<T3> op3)
        => OperationParallelExtensions.WhenAll(op1, op2, op3);

    /// <summary>
    /// Combines multiple operations into a list.
    /// Returns the first failure encountered.
    /// </summary>
    public static Operation<IReadOnlyList<T>> WhenAll<T>(
        params Operation<T>[] ops)
        => OperationParallelExtensions.WhenAll(ops);

    /// <summary>
    /// Asynchronously combines two operations.
    /// </summary>
    public static Task<Operation<(T1, T2)>> WhenAllAsync<T1, T2>(
        Task<Operation<T1>> op1,
        Task<Operation<T2>> op2)
        => OperationParallelExtensions.WhenAllAsync(op1, op2);

    /// <summary>
    /// Asynchronously combines three operations.
    /// </summary>
    public static Task<Operation<(T1, T2, T3)>> WhenAllAsync<T1, T2, T3>(
        Task<Operation<T1>> op1,
        Task<Operation<T2>> op2,
        Task<Operation<T3>> op3)
        => OperationParallelExtensions.WhenAllAsync(op1, op2, op3);

    /// <summary>
    /// Asynchronously combines multiple operations.
    /// </summary>
    public static Task<Operation<IReadOnlyList<T>>> WhenAllAsync<T>(
        params Task<Operation<T>>[] tasks)
        => OperationParallelExtensions.WhenAllAsync(tasks);


    // ------------------------------------------------------------
    // 2. Validation (domain-level sugar)
    // ------------------------------------------------------------

    /// <summary>
    /// Ensures a predicate holds for a successful operation.
    /// If the predicate fails, returns a failure created by <paramref name="errorFactory"/>.
    /// </summary>
    public static Operation<T> Ensure<T>(
        Operation<T> op,
        Func<T, bool> predicate,
        Func<T, Error> errorFactory)
        => op.Ensure(predicate, errorFactory);

    /// <summary>
    /// Asynchronous version of <see cref="Ensure{T}(Operation{T}, Func{T, bool}, Func{T, Error})"/>.
    /// </summary>
    public static Task<Operation<T>> EnsureAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task<bool>> predicateAsync,
        Func<T, Error> errorFactory)
        => op.EnsureAsync(predicateAsync, errorFactory);

    /// <summary>
    /// Ensures a predicate holds, returning a fixed error if it fails.
    /// </summary>
    public static Operation<T> Require<T>(
        Operation<T> op,
        Func<T, bool> predicate,
        Error error)
        => op.Require(predicate, error);

    /// <summary>
    /// Asynchronous version of <see cref="Require{T}(Operation{T}, Func{T, bool}, Error)"/>.
    /// </summary>
    public static Task<Operation<T>> RequireAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task<bool>> predicateAsync,
        Error error)
        => op.RequireAsync(predicateAsync, error);

    /// <summary>
    /// Applies validation rules in order, returning the first failure.
    /// </summary>
    public static Operation<T> Validate<T>(
        Operation<T> op,
        params Func<T, Error?>[] rules)
        => op.Validate(rules);

    /// <summary>
    /// Asynchronous version of <see cref="Validate{T}(Operation{T}, Func{T, Error?>[])"/>.
    /// </summary>
    public static Task<Operation<T>> ValidateAsync<T>(
        Task<Operation<T>> op,
        params Func<T, Task<Error?>>[] rules)
        => op.ValidateAsync(rules);

    /// <summary>
    /// Applies all validation rules and accumulates all errors.
    /// </summary>
    public static Operation<T> ValidateAll<T>(
        Operation<T> op,
        params Func<T, Error?>[] rules)
        => op.ValidateAll(rules);

    /// <summary>
    /// Asynchronous version of <see cref="ValidateAll{T}(Operation{T}, Func{T, Error?>[])"/>.
    /// </summary>
    public static Task<Operation<T>> ValidateAllAsync<T>(
        Task<Operation<T>> op,
        params Func<T, Task<Error?>>[] rules)
        => op.ValidateAllAsync(rules);
}