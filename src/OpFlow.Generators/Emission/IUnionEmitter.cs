// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Generators.Models;

namespace OpFlow.Generators.Emission;

internal interface IUnionEmitter
{
    string Emit(UnionModel union);
}