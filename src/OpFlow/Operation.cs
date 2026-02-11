// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Unions;

namespace OpFlow;

[Union]
public abstract partial record Operation<T>
{
    public sealed partial record Success(T Result) : Operation<T> { }

    public sealed partial record Failure(Error Error) : Operation<T> { }

    // Implicit: T → Success(T)
    public static implicit operator Operation<T>(T value)
        => new Success(value);

    // Implicit: Error → Failure(Error)
    public static implicit operator Operation<T>(Error error)
        => new Failure(error);
}