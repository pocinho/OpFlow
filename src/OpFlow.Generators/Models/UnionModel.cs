// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace OpFlow.Generators.Models;

internal sealed record UnionModel(
    string Namespace,
    string Name,
    Accessibility Accessibility,
    IReadOnlyList<string> Modifiers,
    IReadOnlyList<string> GenericParameters,
    IReadOnlyList<TypeConstraintModel> TypeConstraints,
    IReadOnlyList<CaseField> BaseFields,
    IReadOnlyList<UnionCaseModel> Cases,
    IReadOnlyList<AttributeModel> Attributes,
    string? XmlDocumentation,
    INamedTypeSymbol Symbol, // full semantic symbol
    INamedTypeSymbol? ContainingType
)
{
    public string FullyQualifiedName =>
        $"global::{Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}";

    public bool IsNested => ContainingType is not null;

    public bool IsGeneric => GenericParameters.Count > 0;
}