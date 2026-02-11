// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Unions;

/// <summary>
/// Indicates that a class or struct represents a case in a union type.
/// </summary>
/// <remarks>This attribute is used to annotate union cases in discriminated unions, providing metadata for
/// serialization and pattern matching.</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionCaseAttribute : Attribute
{
}