// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace OpFlow.Generators.Models;

internal sealed record UnionModel(
    string Name,
    string Namespace,
    string FullyQualifiedName,
    IReadOnlyList<string> GenericParameters,
    IReadOnlyList<UnionCaseModel> Cases,
    Accessibility Accessibility
);