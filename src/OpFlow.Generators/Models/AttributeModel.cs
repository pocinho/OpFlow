// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace OpFlow.Generators.Models;

internal sealed record AttributeModel(
    string Name,
    string FullyQualifiedName,
    IReadOnlyList<string> Arguments
);