// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpFlow.Generators.Models;

namespace OpFlow.Generators.Parsing;

internal static class UnionParser
{
    public static UnionModel? TryParse(GeneratorSyntaxContext context)
    {
        if (context.Node is not RecordDeclarationSyntax recordSyntax)
            return null;

        // Must be partial
        if (!recordSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            return null;

        // Must have [Union] — now correctly detects attributes on generic records
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
            Symbol: symbol,
            ContainingType: containingType
        );
    }

    // ─────────────────────────────────────────────────────────────
    // Core checks — FIXED to detect attributes on generic records
    // ─────────────────────────────────────────────────────────────

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

    private static bool HasUnionAttributeOnLists(GeneratorSyntaxContext context, SyntaxList<AttributeListSyntax> lists)
    {
        foreach (AttributeListSyntax list in lists)
        {
            foreach (AttributeSyntax attr in list.Attributes)
            {
                ISymbol? symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
                if (symbol?.ContainingType is INamedTypeSymbol attrType &&
                    attrType.Name == "UnionAttribute" &&
                    attrType.ContainingNamespace.ToDisplayString() == "PP.Toolkit.Operation.Unions")
                {
                    return true;
                }
            }
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

        List<CaseField> fields = new();

        foreach (IParameterSymbol parameter in ctor.Parameters)
        {
            IReadOnlyList<AttributeModel> attributes = ParseAttributes(parameter);
            string? xmlDoc = GetXmlDocumentation(parameter);

            string? defaultValue = parameter.HasExplicitDefaultValue
                ? parameter.ExplicitDefaultValue?.ToString()
                : null;

            fields.Add(new CaseField(
                Name: parameter.Name,
                Type: parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                TypeSymbol: parameter.Type,
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
        List<UnionCaseModel> cases = new();

        foreach (INamedTypeSymbol nested in unionSymbol.GetTypeMembers())
        {
            // Skip compiler-generated nested types
            if (nested.DeclaringSyntaxReferences.Length == 0)
                continue;

            // Must be a record
            if (!nested.IsRecord)
                continue;

            // Must inherit directly from the union type
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
                    IReadOnlyList<AttributeModel> attributes = ParseAttributes(parameter);
                    string? xmlDoc = GetXmlDocumentation(parameter);

                    string? defaultValue = parameter.HasExplicitDefaultValue
                        ? parameter.ExplicitDefaultValue?.ToString()
                        : null;

                    fields.Add(new CaseField(
                        Name: parameter.Name,
                        Type: parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        TypeSymbol: parameter.Type,
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
                XmlDocumentation: xmlDocs,
                CaseTypeSymbol: nested
            ));
        }

        return cases;
    }

    // ─────────────────────────────────────────────────────────────
    // Type constraints
    // ─────────────────────────────────────────────────────────────

    private static IReadOnlyList<TypeConstraintModel> ParseTypeConstraints(RecordDeclarationSyntax recordSyntax)
    {
        List<TypeConstraintModel> list = new();

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
        List<AttributeModel> list = new();

        foreach (AttributeData attr in symbol.GetAttributes())
        {
            INamedTypeSymbol? attrClass = attr.AttributeClass;
            if (attrClass is null)
                continue;

            string name = attrClass.Name;
            string fqName = attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            List<string> args = new();

            foreach (TypedConstant arg in attr.ConstructorArguments)
                args.Add(NormalizeTypedConstant(arg));

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
        return constant.Kind switch
        {
            TypedConstantKind.Primitive => NormalizePrimitive(constant),
            TypedConstantKind.Enum => NormalizeEnum(constant),
            TypedConstantKind.Type => constant.Value is null ? "null" : $"typeof({constant.Value})",
            TypedConstantKind.Array => NormalizeArray(constant),
            TypedConstantKind.Error => "null",
            _ => constant.ToCSharpString()
        };
    }

    private static string NormalizePrimitive(TypedConstant constant)
    {
        return constant.Value is null ? "null" : constant.ToCSharpString();
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