// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace OpFlow.Generators.Models;

internal sealed record CaseField(
    string Name,
    string Type,                       // Fully qualified type name (string form)
    ITypeSymbol TypeSymbol,            // Semantic Roslyn symbol for the type
    string? DefaultValue,
    IReadOnlyList<AttributeModel> Attributes,
    string? XmlDocumentation
);