// Copyright (c) 2026 Paulo Pocinho.

using System;
using System.Collections.Immutable;
using System.Linq;
using OpFlow.Generators.Models;

namespace OpFlow.Generators.Semantics;

internal static class OperationSemanticModelBuilder
{
    public static OperationModel? TryBuild(ImmutableArray<UnionModel> unions)
    {
        // Find the Operation union (if any)
        UnionModel? opUnion = unions.FirstOrDefault(u => u.Name == "Operation");

        if (opUnion is null)
            return null;

        // Validate generic arity for Operation<T>
        if (opUnion.GenericParameters.Count != 1)
        {
            throw new InvalidOperationException(
                $"Operation<T> must have exactly one generic parameter. Found: {opUnion.GenericParameters.Count}");
        }

        // Build semantic model only for Operation<T>
        return new OperationModel(opUnion);
    }
}