// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Unions;

/// <summary>
/// Union Attribute is used to mark a class or struct as a union type.
/// A union type is a type that can hold one of several different types of values,
/// but only one value at a time. This attribute is used by the OpFlow library
/// to generate code for union types.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionAttribute : Attribute
{
}