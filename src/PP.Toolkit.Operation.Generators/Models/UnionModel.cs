// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace PP.Toolkit.Operation.Generators.Models;

internal sealed record UnionModel(
    string Namespace,
    string Modifiers,
    string Name,
    IReadOnlyList<string> GenericParameters,
    IReadOnlyList<UnionCaseModel> Cases,
    IReadOnlyList<CaseField> BaseFields
)
{
    public string FullyQualifiedName =>
        $"global::{Namespace}.{Name}";
}