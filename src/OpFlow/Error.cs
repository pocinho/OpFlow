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
    public abstract string Message { get; }

    /// <summary>
    /// Validation error.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="Fields"></param>
    [UnionCase]
    public sealed partial record Validation(string Reason, IReadOnlyList<string>? Fields = null) : Error
    {
        public override string Message => Reason;
    }

    /// <summary>
    /// Not found error.
    /// </summary>
    /// <param name="Reason"></param>
    [UnionCase]
    public sealed partial record NotFound(string Reason) : Error
    {
        public override string Message => Reason;
    }

    /// <summary>
    /// Unauthorized error.
    /// </summary>
    /// <param name="Reason"></param>
    [UnionCase]
    public sealed partial record Unauthorized(string Reason) : Error
    {
        public override string Message => Reason;
    }

    /// <summary>
    /// Unexpected error. Indicates an exception or an unforeseen failure that occurred during the operation.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="Exception"></param>
    [UnionCase]
    public sealed partial record Unexpected(string Reason, Exception? Exception = null) : Error
    {
        public override string Message => Reason;
    }

    public static Error FromException(Exception ex)
        => new Unexpected(ex.Message, ex);

    public static Error FromMessage(string message)
        => new Validation(message);
}