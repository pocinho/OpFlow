// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow;

using System;
using System.Threading.Tasks;
using OpFlow.Extensions;

/// <summary>
/// Provides a unified, discoverable facade for creating and composing <see cref="Operation{T}"/> values.
/// This class exposes all core OpFlow features, including creation, validation, mapping,
/// binding, parallel composition, LINQ support, and error handling.
/// </summary>
/// <remarks>
/// The <c>Op</c> facade is the recommended entry point for most OpFlow usage.
/// It wraps both extension methods and generated <c>Operation</c> helpers,
/// offering a clean and expressive API.
/// </remarks>
public static class Op
{
    // ------------------------------------------------------------
    // 0. Creation
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
    // 2. Validation
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
    /// Asynchronous version of <see cref="Ensure{T}"/>.
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
    /// Asynchronous version of <see cref="Require{T}"/>.
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
    /// Asynchronous version of <see cref="Validate{T}"/>.
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
    /// Asynchronous version of <see cref="ValidateAll{T}"/>.
    /// </summary>
    public static Task<Operation<T>> ValidateAllAsync<T>(
        Task<Operation<T>> op,
        params Func<T, Task<Error?>>[] rules)
        => op.ValidateAllAsync(rules);


    // ------------------------------------------------------------
    // 3. LINQ / Monad
    // ------------------------------------------------------------

    /// <summary>
    /// Maps a successful value using the specified selector.
    /// </summary>
    public static Operation<TResult> Select<T, TResult>(
        Operation<T> op,
        Func<T, TResult> selector)
        => op.Select(selector);

    /// <summary>
    /// Asynchronous version of <see cref="Select{T, TResult}"/>.
    /// </summary>
    public static Task<Operation<TResult>> SelectAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> selector)
        => op.SelectAsync(selector);

    /// <summary>
    /// Chains two operations and projects their results.
    /// </summary>
    public static Operation<TResult> SelectMany<T, TIntermediate, TResult>(
        Operation<T> op,
        Func<T, Operation<TIntermediate>> bind,
        Func<T, TIntermediate, TResult> project)
        => op.SelectMany(bind, project);

    /// <summary>
    /// Asynchronous version of <see cref="SelectMany{T, TIntermediate, TResult}"/>.
    /// </summary>
    public static Task<Operation<TResult>> SelectManyAsync<T, TIntermediate, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<Operation<TIntermediate>>> bindAsync,
        Func<T, TIntermediate, TResult> project)
        => op.SelectManyAsync(bindAsync, project);

    /// <summary>
    /// LINQ-friendly async SelectMany wrapper.
    /// </summary>
    public static Task<Operation<TResult>> SelectMany<T, TIntermediate, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, Task<Operation<TIntermediate>>> bindAsync,
        Func<T, TIntermediate, TResult> project)
        => SelectManyAsync(opTask, bindAsync, project);

    /// <summary>
    /// LINQ-friendly async Select wrapper.
    /// </summary>
    public static Task<Operation<TResult>> Select<T, TResult>(
        this Task<Operation<T>> opTask,
        Func<T, TResult> selector)
        => opTask.SelectAsync(x => Task.FromResult(selector(x)));


    // ------------------------------------------------------------
    // 4. Map / Bind
    // ------------------------------------------------------------

    /// <summary>
    /// Maps a successful value to another value.
    /// </summary>
    public static Operation<TResult> Map<T, TResult>(
        Operation<T> op,
        Func<T, TResult> map)
        => op.Map(map);

    /// <summary>
    /// Asynchronous version of <see cref="Map{T, TResult}"/>.
    /// </summary>
    public static Task<Operation<TResult>> MapAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> mapAsync)
        => op.MapAsync(mapAsync);

    /// <summary>
    /// Chains two operations.
    /// </summary>
    public static Operation<TResult> Bind<T, TResult>(
        Operation<T> op,
        Func<T, Operation<TResult>> bind)
        => op.Bind(bind);

    /// <summary>
    /// Asynchronous version of <see cref="Bind{T, TResult}"/>.
    /// </summary>
    public static Task<Operation<TResult>> BindAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<Operation<TResult>>> bindAsync)
        => op.BindAsync(bindAsync);


    // ------------------------------------------------------------
    // 5. Tap / OnSuccess / OnFailure
    // ------------------------------------------------------------

    /// <summary>
    /// Executes a side effect when the operation succeeds.
    /// </summary>
    public static Operation<T> Tap<T>(
        Operation<T> op,
        Action<T> action)
        => op.Tap(action);

    /// <summary>
    /// Executes a side effect when the operation fails.
    /// </summary>
    public static Operation<T> TapError<T>(
        Operation<T> op,
        Action<Error> action)
        => op.TapError(action);

    /// <summary>
    /// Asynchronous version of <see cref="Tap{T}"/>.
    /// </summary>
    public static Task<Operation<T>> TapAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task> actionAsync)
        => op.TapAsync(actionAsync);

    /// <summary>
    /// Asynchronous version of <see cref="TapError{T}"/>.
    /// </summary>
    public static Task<Operation<T>> TapErrorAsync<T>(
        Task<Operation<T>> op,
        Func<Error, Task> actionAsync)
        => op.TapErrorAsync(actionAsync);

    /// <summary>
    /// Executes an action when the operation succeeds.
    /// </summary>
    public static Operation<T> OnSuccess<T>(
        Operation<T> op,
        Action<T> action)
        => op.OnSuccess(action);

    /// <summary>
    /// Executes an action when the operation fails.
    /// </summary>
    public static Operation<T> OnFailure<T>(
        Operation<T> op,
        Action<Error> action)
        => op.OnFailure(action);


    // ------------------------------------------------------------
    // 6. Recover
    // ------------------------------------------------------------

    /// <summary>
    /// Provides a fallback value when the operation fails.
    /// </summary>
    public static Operation<T> Recover<T>(
        Operation<T> op,
        Func<Error, T> fallback)
        => op.Recover(fallback);

    /// <summary>
    /// Asynchronous version of <see cref="Recover{T}"/>.
    /// </summary>
    public static Task<Operation<T>> RecoverAsync<T>(
        Task<Operation<T>> op,
        Func<Error, Task<T>> fallbackAsync)
        => op.RecoverAsync(fallbackAsync);


    // ------------------------------------------------------------
    // 7. Match
    // ------------------------------------------------------------

    /// <summary>
    /// Pattern-matches the operation, returning a value based on success or failure.
    /// </summary>
    public static TResult Match<T, TResult>(
        Operation<T> op,
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
        => op.Match(onSuccess, onFailure);

    /// <summary>
    /// Pattern-matches the operation, executing an action based on success or failure.
    /// </summary>
    public static void Match<T>(
        Operation<T> op,
        Action<T> onSuccess,
        Action<Error> onFailure)
        => op.Match(onSuccess, onFailure);

    /// <summary>
    /// Asynchronous version of <see cref="Match{T, TResult}"/>.
    /// </summary>
    public static Task<TResult> MatchAsync<T, TResult>(
        Task<Operation<T>> op,
        Func<T, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure)
        => op.MatchAsync(onSuccess, onFailure);

    /// <summary>
    /// Asynchronous version of <see cref="Match{T}"/>.
    /// </summary>
    public static Task MatchAsync<T>(
        Task<Operation<T>> op,
        Func<T, Task> onSuccess,
        Func<Error, Task> onFailure)
        => op.MatchAsync(onSuccess, onFailure);
}