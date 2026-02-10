// Copyright (c) 2026 Paulo Pocinho.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using PP.Toolkit.Operation.Generators.Models;

namespace PP.Toolkit.Operation.Generators.Semantics;

internal sealed class OperationModel
{
    public UnionModel Union { get; }

    public UnionCaseModel SuccessCase { get; }
    public UnionCaseModel FailureCase { get; }

    public CaseField ResultField { get; }
    public CaseField ErrorField { get; }

    public string GenericParameter { get; }
    public string FullyQualifiedName => Union.FullyQualifiedName;
    public string Namespace => Union.Namespace;
    public Accessibility Accessibility => Union.Accessibility;

    public OperationModel(UnionModel union)
    {
        Union = union;

        // Validate generic arity
        if (union.GenericParameters.Count != 1)
            throw new InvalidOperationException(
                $"Operation<T> must have exactly one generic parameter. Found: {union.GenericParameters.Count}");

        GenericParameter = union.GenericParameters[0];

        // Validate case count
        if (union.Cases.Count != 2)
            throw new InvalidOperationException(
                $"Operation<T> must have exactly two union cases. Found: {union.Cases.Count}");

        // Identify cases
        SuccessCase = union.Cases.FirstOrDefault(c => c.Name == "Success")
            ?? throw new InvalidOperationException("Operation<T> must contain a 'Success' case.");

        FailureCase = union.Cases.FirstOrDefault(c => c.Name == "Failure")
            ?? throw new InvalidOperationException("Operation<T> must contain a 'Failure' case.");

        // Validate Success(Result)
        if (SuccessCase.Fields.Count != 1)
            throw new InvalidOperationException(
                "Success case must have exactly one field: Result.");

        ResultField = SuccessCase.Fields[0];

        // Validate Failure(Error)
        if (FailureCase.Fields.Count != 1)
            throw new InvalidOperationException(
                "Failure case must have exactly one field: Error.");

        ErrorField = FailureCase.Fields[0];

        // Validate Error type
        if (!ErrorField.Type.EndsWith("Error") &&
            !ErrorField.Type.Contains(".Error"))
        {
            throw new InvalidOperationException(
                $"Failure.Error must be of type Error. Found: {ErrorField.Type}");
        }
    }

    // Convenience helpers
    public string SuccessCaseFQN => SuccessCase.FullyQualifiedName(Namespace, Union.Name);
    public string FailureCaseFQN => FailureCase.FullyQualifiedName(Namespace, Union.Name);
}