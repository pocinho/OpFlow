// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace PP.Toolkit.Operation.Generators.Models;

internal sealed record UnionCaseModel(
    string Name,
    IReadOnlyList<CaseField> Fields,
    IReadOnlyList<string> GenericParameters,
    string? BaseTypeName = null
)
{
    public string FullyQualifiedName(string unionNamespace, string unionName) =>
        $"global::{unionNamespace}.{unionName}.{Name}";
}