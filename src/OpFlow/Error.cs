// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Unions;

namespace OpFlow;

/// <summary>
/// Error represents a failure case in an operation. It is designed
/// as a discriminated union to capture different kinds of errors
/// that can occur during the execution of an operation.
/// </summary>
[Union]
public abstract partial record Error
{
    /// <summary>
    /// Validation error.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Fields"></param>
    [UnionCase]
    public sealed partial record Validation(string Message, IReadOnlyList<string>? Fields = null)
        : Error;

    /// <summary>
    /// Not found error.
    /// </summary>
    /// <param name="Message"></param>
    [UnionCase]
    public sealed partial record NotFound(string Message)
        : Error;

    /// <summary>
    /// Unauthorized error.
    /// </summary>
    /// <param name="Message"></param>
    [UnionCase]
    public sealed partial record Unauthorized(string Message)
        : Error;

    /// <summary>
    /// Unexpected error. Indicates an exception or an unforeseen failure that occurred during the operation.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Exception"></param>
    [UnionCase]
    public sealed partial record Unexpected(string Message, Exception? Exception = null)
        : Error;
}