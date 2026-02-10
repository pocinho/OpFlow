// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace PP.Toolkit.Operation.Generators.Models;

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
    INamedTypeSymbol? ContainingType
)
{
    public string FullyQualifiedName =>
        ContainingType is null
            ? $"global::{Namespace}.{Name}"
            : $"global::{ContainingType.ToDisplayString()}.{Name}";
}