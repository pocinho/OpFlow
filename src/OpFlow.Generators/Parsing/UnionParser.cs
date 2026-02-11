// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpFlow.Generators.Models;

namespace OpFlow.Generators.Parsing;

internal static class UnionParser
{
    public static UnionModel? TryParse(GeneratorSyntaxContext context)
    {
        if (context.Node is not RecordDeclarationSyntax recordSyntax)
            return null;

        if (!HasUnionAttribute(context, recordSyntax))
            return null;

        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(recordSyntax) as INamedTypeSymbol;
        if (symbol is null)
            return null;

        string ns = symbol.ContainingNamespace?.ToDisplayString() ?? "";
        string name = symbol.Name;

        List<string> genericParams = symbol.TypeParameters
            .Select(tp => tp.Name)
            .ToList();

        IReadOnlyList<UnionCaseModel> cases = ParseCases(symbol);

        string fqn = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new UnionModel(
            Name: name,
            Namespace: ns,
            FullyQualifiedName: fqn,
            GenericParameters: genericParams,
            Cases: cases,
            Accessibility: symbol.DeclaredAccessibility
        );
    }

    private static bool HasUnionAttribute(GeneratorSyntaxContext context, RecordDeclarationSyntax record)
    {
        foreach (AttributeListSyntax list in record.AttributeLists)
        {
            foreach (AttributeSyntax attr in list.Attributes)
            {
                ISymbol? symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
                if (symbol?.ContainingType is INamedTypeSymbol attrType &&
                    attrType.Name == "UnionAttribute" &&
                    attrType.ContainingNamespace.ToDisplayString() == "OpFlow.Unions")
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IReadOnlyList<UnionCaseModel> ParseCases(INamedTypeSymbol unionSymbol)
    {
        List<UnionCaseModel> cases = new();

        foreach (INamedTypeSymbol nested in unionSymbol.GetTypeMembers())
        {
            if (nested.DeclaringSyntaxReferences.Length == 0)
                continue;

            if (!nested.IsRecord)
                continue;

            if (!SymbolEqualityComparer.Default.Equals(nested.BaseType, unionSymbol))
                continue;

            IMethodSymbol? ctor = nested.InstanceConstructors
                .Where(c => !c.IsImplicitlyDeclared)
                .OrderBy(c => c.Parameters.Length)
                .LastOrDefault();

            List<CaseField> fields = new();

            if (ctor is not null)
            {
                foreach (IParameterSymbol parameter in ctor.Parameters)
                {
                    string typeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    fields.Add(new CaseField(
                        Name: parameter.Name,
                        Type: typeName,
                        TypeSymbol: parameter.Type
                    ));
                }
            }

            List<string> genericParams = nested.TypeParameters
                .Select(tp => tp.Name)
                .ToList();

            string? baseTypeName = nested.BaseType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            cases.Add(new UnionCaseModel(
                Name: nested.Name,
                Fields: fields,
                GenericParameters: genericParams,
                BaseTypeName: baseTypeName,
                Accessibility: nested.DeclaredAccessibility,
                CaseTypeSymbol: nested
            ));
        }

        return cases;
    }
}