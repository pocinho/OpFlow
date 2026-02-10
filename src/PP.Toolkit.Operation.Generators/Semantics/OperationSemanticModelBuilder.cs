// Copyright (c) 2026 Paulo Pocinho.

using System;
using System.Collections.Generic;
using System.Linq;
using PP.Toolkit.Operation.Generators.Models;

namespace PP.Toolkit.Operation.Generators.Semantics;

internal static class OperationSemanticModelBuilder
{
    public static OperationModel? TryBuild(IEnumerable<UnionModel> unions)
    {
        // Find the union named "Operation"
        var opUnion = unions.FirstOrDefault(u => u.Name == "Operation");

        if (opUnion is null)
            return null; // Operation<T> not present in this compilation

        try
        {
            return new OperationModel(opUnion);
        }
        catch (Exception)
        {
            // If the union exists but is malformed, we return null
            // Emitters can decide whether to emit diagnostics or skip generation
            return null;
        }
    }
}