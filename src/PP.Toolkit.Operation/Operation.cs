// Copyright (c) 2026 Paulo Pocinho.

using PP.Toolkit.Operation.Unions;

namespace PP.Toolkit.Operation;

[Union]
public abstract partial record Operation<T>
{
    [UnionCase]
    public sealed partial record Success(T Result) : Operation<T>;

    [UnionCase]
    public sealed partial record Failure(Error Error) : Operation<T>;

    // Implicit: T → Success(T)
    public static implicit operator Operation<T>(T value)
        => new Success(value);

    // Implicit: Error → Failure(Error)
    public static implicit operator Operation<T>(Error error)
        => new Failure(error);
}

public static class Operation
{
    public static Operation<T> Ok<T>(T value)
        => new Operation<T>.Success(value);

    public static Operation<T> Fail<T>(Error error)
        => new Operation<T>.Failure(error);

    public static Operation<T> Validation<T>(string message, params string[] fields)
        => Fail<T>(new Error.Validation(message, fields));

    public static Operation<T> NotFound<T>(string message)
        => Fail<T>(new Error.NotFound(message));

    public static Operation<T> Unauthorized<T>(string message)
        => Fail<T>(new Error.Unauthorized(message));

    public static Operation<T> Unexpected<T>(string message, Exception? ex = null)
        => Fail<T>(new Error.Unexpected(message, ex));
}