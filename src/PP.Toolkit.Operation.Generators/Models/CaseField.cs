// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Generators.Models;

internal sealed record CaseField(
    string Name,
    string Type,
    string? DefaultValue = null
);