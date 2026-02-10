// Copyright (c) 2026 Paulo Pocinho.

using PP.Toolkit.Operation.Generators.Semantics;

namespace PP.Toolkit.Operation.Generators.Emission;

internal interface IOperationEmitter
{
    string FileName(OperationModel op);
    string Emit(OperationModel op);
}