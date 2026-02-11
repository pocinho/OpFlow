// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Generators.Semantics;

namespace OpFlow.Generators.Emission;

internal interface IOperationEmitter
{
    string FileName(OperationModel op);
    string Emit(OperationModel op);
}