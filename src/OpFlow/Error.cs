// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Unions;

namespace OpFlow;

[Union]
public abstract partial record Error
{
    [UnionCase]
    public sealed partial record Validation(string Message, IReadOnlyList<string>? Fields = null)
        : Error;

    [UnionCase]
    public sealed partial record NotFound(string Message)
        : Error;

    [UnionCase]
    public sealed partial record Unauthorized(string Message)
        : Error;

    [UnionCase]
    public sealed partial record Unexpected(string Message, Exception? Exception = null)
        : Error;
}