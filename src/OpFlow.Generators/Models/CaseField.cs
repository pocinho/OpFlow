// Copyright (c) 2026 Paulo Pocinho.

using Microsoft.CodeAnalysis;

namespace OpFlow.Generators.Models;

internal sealed record CaseField(
    string Name,
    string Type,
    ITypeSymbol TypeSymbol
);