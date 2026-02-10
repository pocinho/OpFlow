// Copyright (c) 2026 Paulo Pocinho.

using PP.Toolkit.Operation.Generators.Models;

namespace PP.Toolkit.Operation.Generators.Emission;

internal interface IUnionEmitter
{
    string Emit(UnionModel union);
}