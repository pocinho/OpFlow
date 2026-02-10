// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;

namespace PP.Toolkit.Operation.Generators.Models;

internal sealed record CaseField(
    string Name,
    string Type,
    string? DefaultValue,
    IReadOnlyList<AttributeModel> Attributes,
    string? XmlDocumentation
);