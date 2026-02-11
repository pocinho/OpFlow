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
    INamedTypeSymbol CaseTypeSymbol
);