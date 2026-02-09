// Copyright (c) 2026 Paulo Pocinho.

using PP.Toolkit.Operation.Unions;

namespace PP.Toolkit.Operation;

[Union]
public abstract partial record Error(string Message)
{
    [UnionCase]
    public sealed partial record Validation(string Message, IReadOnlyList<string>? Fields = null)
        : Error(Message);

    [UnionCase]
    public sealed partial record NotFound(string Message)
        : Error(Message);

    [UnionCase]
    public sealed partial record Unauthorized(string Message)
        : Error(Message);

    [UnionCase]
    public sealed partial record Unexpected(string Message, Exception? Exception = null)
        : Error(Message);
}