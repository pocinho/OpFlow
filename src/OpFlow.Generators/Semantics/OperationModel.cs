// Copyright (c) 2026 Paulo Pocinho.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using OpFlow.Generators.Models;

namespace OpFlow.Generators.Semantics;

internal sealed class OperationModel
{
    public UnionModel Union { get; }

    public UnionCaseModel SuccessCase { get; }
    public UnionCaseModel FailureCase { get; }

    public CaseField ResultField { get; }
    public CaseField ErrorField { get; }

    public string GenericParameter { get; }
    public string FullyQualifiedName => StripGlobal(Union.FullyQualifiedName);
    public string Namespace => Union.Namespace;
    public Accessibility Accessibility => Union.Accessibility;

    private static readonly SymbolDisplayFormat FqnFormat =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public OperationModel(UnionModel union)
    {
        Union = union;

        if (union.GenericParameters.Count != 1)
            throw new InvalidOperationException("Operation<T> must have exactly one generic parameter.");

        GenericParameter = union.GenericParameters[0];

        if (union.Cases.Count != 2)
            throw new InvalidOperationException("Operation<T> must have exactly two union cases.");

        SuccessCase = union.Cases.FirstOrDefault(c => c.Name == "Success")
            ?? throw new InvalidOperationException("Operation<T> must contain a 'Success' case.");

        FailureCase = union.Cases.FirstOrDefault(c => c.Name == "Failure")
            ?? throw new InvalidOperationException("Operation<T> must contain a 'Failure' case.");

        if (SuccessCase.Fields.Count != 1)
            throw new InvalidOperationException("Success case must have exactly one field: Result.");

        ResultField = SuccessCase.Fields[0];

        if (FailureCase.Fields.Count != 1)
            throw new InvalidOperationException("Failure case must have exactly one field: Error.");

        ErrorField = FailureCase.Fields[0];

        if (!IsErrorType(ErrorField.TypeSymbol))
            throw new InvalidOperationException(
                $"Failure.Error must be of type Error (or derive from it). Found: {ErrorField.Type}");
    }

    public string SuccessCaseFQN =>
        SuccessCase.CaseTypeSymbol.ToDisplayString(FqnFormat);   // e.g. global::OpFlow.Operation<T>.Success

    public string FailureCaseFQN =>
        FailureCase.CaseTypeSymbol.ToDisplayString(FqnFormat);   // e.g. global::OpFlow.Operation<T>.Failure

    public string ErrorTypeFQN =>
        StripGlobal(ErrorField.Type);

    private static string StripGlobal(string fqn) =>
        fqn.StartsWith("global::", StringComparison.Ordinal)
            ? fqn.Substring("global::".Length)
            : fqn;

    private static bool IsErrorType(ITypeSymbol? type)
    {
        while (type is not null)
        {
            if (type.Name == "Error" &&
                type.ContainingNamespace?.ToDisplayString() == "OpFlow")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }
}