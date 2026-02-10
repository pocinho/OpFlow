// Copyright (c) 2026 Paulo Pocinho.

using PP.Toolkit.Operation.Unions;

namespace PP.Toolkit.Operation; // PP.Toolkit.Operation is a separate project from PP.Toolkit.Operation.Generators, which contains the source generator.

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

public static partial class Operation
{
    public static Operation<T> Ok<T>(T value)
        => new Operation<T>.Success(value);
}