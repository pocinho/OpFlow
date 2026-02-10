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

        Accessibility accessibility = symbol.DeclaredAccessibility;
        List<string> modifiers = recordSyntax.Modifiers.Select(m => m.Text).ToList();

        List<string> genericParams = symbol.TypeParameters
            .Select(tp => tp.Name)
            .ToList();

        IReadOnlyList<TypeConstraintModel> typeConstraints = ParseTypeConstraints(recordSyntax);

        IReadOnlyList<CaseField> baseFields = ParseBaseFields(symbol);

        IReadOnlyList<UnionCaseModel> cases = ParseCases(symbol);

        IReadOnlyList<AttributeModel> attributes = ParseAttributes(symbol);
        string? xmlDoc = GetXmlDocumentation(symbol);

        INamedTypeSymbol? containingType = symbol.ContainingType;

        return new UnionModel(
            Namespace: ns,
            Name: name,
            Accessibility: accessibility,
            Modifiers: modifiers,
            GenericParameters: genericParams,
            TypeConstraints: typeConstraints,
            BaseFields: baseFields,
            Cases: cases,
            Attributes: attributes,
            XmlDocumentation: xmlDoc,
            ContainingType: containingType
        );
    }

    // ─────────────────────────────────────────────────────────────
    // Core checks
    // ─────────────────────────────────────────────────────────────

    private static bool HasUnionAttribute(GeneratorSyntaxContext context, RecordDeclarationSyntax record)
    {
        foreach (AttributeListSyntax list in record.AttributeLists)
            foreach (AttributeSyntax attr in list.Attributes)
            {
                ISymbol? symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
                if (symbol?.ContainingType?.Name == "UnionAttribute")
                    return true;
            }

        return false;
    }

    // ─────────────────────────────────────────────────────────────
    // Base fields (primary ctor of the union)
    // ─────────────────────────────────────────────────────────────

    private static IReadOnlyList<CaseField> ParseBaseFields(INamedTypeSymbol unionSymbol)
    {
        IMethodSymbol? ctor = unionSymbol.InstanceConstructors
            .FirstOrDefault(c => !c.IsImplicitlyDeclared);

        if (ctor is null)
            return new List<CaseField>();

        List<CaseField> fields = new List<CaseField>();

        foreach (IParameterSymbol? parameter in ctor.Parameters)
        {
            IReadOnlyList<AttributeModel> attributes = ParseAttributes(parameter);
            string? xmlDoc = GetXmlDocumentation(parameter);

            string? defaultValue = parameter.HasExplicitDefaultValue
                ? parameter.ExplicitDefaultValue?.ToString()
                : null;

            fields.Add(new CaseField(
                Name: parameter.Name,
                Type: parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                DefaultValue: defaultValue,
                Attributes: attributes,
                XmlDocumentation: xmlDoc
            ));
        }

        return fields;
    }

    // ─────────────────────────────────────────────────────────────
    // Union cases
    // ─────────────────────────────────────────────────────────────

    private static IReadOnlyList<UnionCaseModel> ParseCases(INamedTypeSymbol unionSymbol)
    {
        List<UnionCaseModel> cases = new List<UnionCaseModel>();

        foreach (INamedTypeSymbol? nested in unionSymbol.GetTypeMembers())
        {
            if (!HasUnionCaseAttribute(nested))
                continue;

            IMethodSymbol? ctor = nested.InstanceConstructors
                .Where(c => !c.IsImplicitlyDeclared)
                .OrderBy(c => c.Parameters.Length)
                .LastOrDefault();

            List<CaseField> fields = new List<CaseField>();

            if (ctor is not null)
            {
                foreach (IParameterSymbol? parameter in ctor.Parameters)
                {
                    IReadOnlyList<AttributeModel> attributes = ParseAttributes(parameter);
                    string? xmlDoc = GetXmlDocumentation(parameter);

                    string? defaultValue = parameter.HasExplicitDefaultValue
                        ? parameter.ExplicitDefaultValue?.ToString()
                        : null;

                    fields.Add(new CaseField(
                        Name: parameter.Name,
                        Type: parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        DefaultValue: defaultValue,
                        Attributes: attributes,
                        XmlDocumentation: xmlDoc
                    ));
                }
            }

            List<string> genericParams = nested.TypeParameters
                .Select(tp => tp.Name)
                .ToList();

            string? baseTypeName = nested.BaseType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            IReadOnlyList<AttributeModel> attribs = ParseAttributes(nested);
            string? xmlDocs = GetXmlDocumentation(nested);
            Accessibility accessibility = nested.DeclaredAccessibility;

            cases.Add(new UnionCaseModel(
                Name: nested.Name,
                Fields: fields,
                GenericParameters: genericParams,
                BaseTypeName: baseTypeName,
                Accessibility: accessibility,
                Attributes: attribs,
                XmlDocumentation: xmlDocs
            ));
        }

        return cases;
    }

    private static bool HasUnionCaseAttribute(INamedTypeSymbol nested)
        => nested.GetAttributes().Any(a => a.AttributeClass?.Name == "UnionCaseAttribute");

    // ─────────────────────────────────────────────────────────────
    // Type constraints
    // ─────────────────────────────────────────────────────────────

    private static IReadOnlyList<TypeConstraintModel> ParseTypeConstraints(RecordDeclarationSyntax recordSyntax)
    {
        List<TypeConstraintModel> list = new List<TypeConstraintModel>();

        foreach (TypeParameterConstraintClauseSyntax clause in recordSyntax.ConstraintClauses)
        {
            string typeParam = clause.Name.Identifier.Text;

            List<string> constraints = clause.Constraints
                .Select(c => c.ToString())
                .ToList();

            list.Add(new TypeConstraintModel(
                TypeParameter: typeParam,
                Constraints: constraints
            ));
        }

        return list;
    }

    // ─────────────────────────────────────────────────────────────
    // Attributes
    // ─────────────────────────────────────────────────────────────

    private static IReadOnlyList<AttributeModel> ParseAttributes(ISymbol symbol)
    {
        List<AttributeModel> list = new List<AttributeModel>();

        foreach (AttributeData? attr in symbol.GetAttributes())
        {
            INamedTypeSymbol? attrClass = attr.AttributeClass;
            if (attrClass is null)
                continue;

            string name = attrClass.Name;
            string fqName = attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            List<string> args = new List<string>();

            // Positional constructor arguments
            foreach (TypedConstant arg in attr.ConstructorArguments)
                args.Add(NormalizeTypedConstant(arg));

            // Named arguments
            foreach (KeyValuePair<string, TypedConstant> kvp in attr.NamedArguments)
            {
                string key = kvp.Key;
                string value = NormalizeTypedConstant(kvp.Value);
                args.Add($"{key} = {value}");
            }

            list.Add(new AttributeModel(
                Name: name,
                FullyQualifiedName: fqName,
                Arguments: args
            ));
        }

        return list;
    }

    private static string NormalizeTypedConstant(TypedConstant constant)
    {
        switch (constant.Kind)
        {
            case TypedConstantKind.Primitive:
                return NormalizePrimitive(constant);

            case TypedConstantKind.Enum:
                return NormalizeEnum(constant);

            case TypedConstantKind.Type:
                return constant.Value is null
                    ? "null"
                    : $"typeof({constant.Value})";

            case TypedConstantKind.Array:
                return NormalizeArray(constant);

            case TypedConstantKind.Error:
                return "null";

            default:
                return constant.ToCSharpString();
        }
    }

    private static string NormalizePrimitive(TypedConstant constant)
    {
        if (constant.Value is null)
            return "null";

        return constant.ToCSharpString();
    }

    private static string NormalizeEnum(TypedConstant constant)
    {
        if (constant.Type is null || constant.Value is null)
            return "default";

        string enumType = constant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string enumValue = constant.Value.ToString();

        return $"{enumType}.{enumValue}";
    }

    private static string NormalizeArray(TypedConstant constant)
    {
        if (constant.Values.IsDefaultOrEmpty)
            return "new[] { }";

        List<string> items = constant.Values
            .Select(NormalizeTypedConstant)
            .ToList();

        return $"new[] {{ {string.Join(", ", items)} }}";
    }

    // ─────────────────────────────────────────────────────────────
    // XML documentation
    // ─────────────────────────────────────────────────────────────

    private static string? GetXmlDocumentation(ISymbol symbol)
    {
        string? xml = symbol.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(xml) ? null : xml;
    }
}