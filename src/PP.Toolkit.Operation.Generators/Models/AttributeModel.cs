// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace PP.Toolkit.Operation.Generators.Models;

internal sealed record AttributeModel(
    string Name,
    string FullyQualifiedName,
    IReadOnlyList<string> Arguments
);