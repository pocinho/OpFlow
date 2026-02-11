// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Unions;

namespace OpFlow;

[Union]
public abstract partial record Operation<T>
{
    public sealed partial record Success(T Result) : Operation<T>
    {
        public override string ToString() => $"Success: {Result}";
    }

    public sealed partial record Failure(Error Error) : Operation<T>
    {
        public override string ToString() => $"Failure: {Error}";
    }

    public static implicit operator Operation<T>(Error error)
        => new Failure(error);

    public static implicit operator Operation<T>(Exception ex)
        => new Failure(new Error.Unexpected(ex.Message, ex));

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