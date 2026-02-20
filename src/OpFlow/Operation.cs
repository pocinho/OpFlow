// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Unions;

namespace OpFlow;

/// <summary>
/// Represents the outcome of an operation, which can be either a success containing a result of type T or a failure
/// containing an error.
/// </summary>
/// <remarks>This abstract record provides a unified way to handle operation results, distinguishing between
/// successful and failed outcomes. Use the Success record to encapsulate a successful result, and the Failure record to
/// encapsulate an error. Implicit conversions are available to create a Failure from an Error or an Exception,
/// simplifying error handling.</remarks>
/// <typeparam name="T">The type of the result returned when the operation succeeds.</typeparam>
[Union]
public abstract partial record Operation<T>
{
    /// <summary>
    /// Successful outcome of an operation, containing the result of type T.
    /// </summary>
    public sealed partial record Success : Operation<T>
    {
        public T Result { get; }

        internal Success(T result) => Result = result;

        public override string ToString() => $"Success: {Result}";
    }

    /// <summary>
    /// Failed outcome of an operation, containing an error. The error can be created from an Error instance or an Exception.
    /// </summary>
    public sealed partial record Failure : Operation<T>
    {
        public Error Error { get; }

        internal Failure(Error error) => Error = error;

        public override string ToString() => $"Failure: {Error}";
    }


    public override string ToString()
    {
        return this switch
        {
            Success s => s.ToString(),
            Failure f => f.ToString(),
            _ => throw new InvalidOperationException()
        };
    }
}