// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace OpFlow.Generators.Models;

internal sealed record UnionCaseModel(
    string Name,
    IReadOnlyList<CaseField> Fields,
    IReadOnlyList<string> GenericParameters,
    string? BaseTypeName,
    Accessibility Accessibility,
    IReadOnlyList<AttributeModel> Attributes,
    string? XmlDocumentation,
    INamedTypeSymbol CaseTypeSymbol
)
{
    public string FullyQualifiedName =>
        $"global::{CaseTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}";

    public string Namespace =>
        CaseTypeSymbol.ContainingNamespace.ToDisplayString();
}