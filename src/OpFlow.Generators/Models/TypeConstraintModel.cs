// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace OpFlow.Generators.Models;

internal sealed record TypeConstraintModel(
    string TypeParameter,
    IReadOnlyList<string> Constraints
);