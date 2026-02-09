// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PP.Toolkit.Operation.Generators.Models;

namespace PP.Toolkit.Operation.Generators.Parsing;

internal static class UnionParser
{
    public static UnionModel? TryParse(GeneratorSyntaxContext context)
    {
        if (context.Node is not RecordDeclarationSyntax recordSyntax)
            return null;

        // Must be partial
        if (!recordSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            return null;

        // Must have [Union]
        if (!HasUnionAttribute(context, recordSyntax))
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(recordSyntax) is not INamedTypeSymbol symbol)
            return null;

        string ns = symbol.ContainingNamespace.ToDisplayString();
        string name = symbol.Name;

        // Generic parameters of the union
        List<string> genericParams = symbol.TypeParameters
            .Select(tp => tp.Name)
            .ToList();

        // Base constructor fields (e.g., Error(string Message))
        IReadOnlyList<CaseField> baseFields = ParseBaseFields(symbol);

        // Nested union cases
        IReadOnlyList<UnionCaseModel> cases = ParseCases(symbol);

        return new UnionModel(
            Namespace: ns,
            Modifiers: string.Join(" ", recordSyntax.Modifiers.Select(m => m.Text)),
            Name: name,
            GenericParameters: genericParams,
            Cases: cases,
            BaseFields: baseFields
        );
    }

    private static bool HasUnionAttribute(GeneratorSyntaxContext context, RecordDeclarationSyntax record)
    {
        foreach (AttributeListSyntax list in record.AttributeLists)
            foreach (AttributeSyntax attr in list.Attributes)
            {
                ISymbol? symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
                if (symbol?.ContainingType.Name == "UnionAttribute")
                    return true;
            }

        return false;
    }

    private static IReadOnlyList<CaseField> ParseBaseFields(INamedTypeSymbol unionSymbol)
    {
        // Find the primary constructor of the union
        IMethodSymbol? ctor = unionSymbol.InstanceConstructors
            .FirstOrDefault(c => !c.IsImplicitlyDeclared);

        return ctor?.Parameters
            .Select(p => new CaseField(p.Name, p.Type.ToDisplayString()))
            .ToList()
            ?? new List<CaseField>();
    }

    private static IReadOnlyList<UnionCaseModel> ParseCases(INamedTypeSymbol unionSymbol)
    {
        List<UnionCaseModel> cases = new List<UnionCaseModel>();

        foreach (INamedTypeSymbol? nested in unionSymbol.GetTypeMembers())
        {
            bool hasUnionCase = nested
                .GetAttributes()
                .Any(a => a.AttributeClass?.Name == "UnionCaseAttribute");

            if (!hasUnionCase)
                continue;

            // Pick the primary constructor (positional records)
            IMethodSymbol? ctor = nested.InstanceConstructors
                .Where(c => !c.IsImplicitlyDeclared)
                .OrderBy(c => c.Parameters.Length)
                .LastOrDefault();

            List<CaseField> fields = ctor?.Parameters
                                         .Select(p => new CaseField(
                                             Name: p.Name,
                                             Type: p.Type.ToDisplayString(),
                                             DefaultValue: p.HasExplicitDefaultValue
                                                 ? p.ExplicitDefaultValue?.ToString()
                                                 : null
                                         ))
                                         .ToList()
                                     ?? new List<CaseField>();

            List<string> genericParams = nested.TypeParameters
                .Select(tp => tp.Name)
                .ToList();

            string? baseTypeName = nested.BaseType?.ToDisplayString();

            cases.Add(new UnionCaseModel(
                Name: nested.Name,
                Fields: fields,
                GenericParameters: genericParams,
                BaseTypeName: baseTypeName
            ));
        }

        return cases;
    }
}