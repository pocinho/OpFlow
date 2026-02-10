// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PP.Toolkit.Operation.Generators.Emission.Emitters;
using PP.Toolkit.Operation.Generators.Parsing;
using PP.Toolkit.Operation.Generators.Semantics;

namespace PP.Toolkit.Operation.Generators.Generator;

[Generator]
public sealed class OperationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Parse all unions
        var unions = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (ctx, _) => UnionParser.TryParse(ctx))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!)
            .Collect();

        // 2. Register the main output
        context.RegisterSourceOutput(unions, static (spc, unionList) =>
        {
            // Build semantic OperationModel (if present)
            OperationModel? operation = OperationSemanticModelBuilder.TryBuild(unionList);

            foreach (var union in unionList)
            {
                bool isError = union.Name == "Error";
                bool isOperation = union.Name == "Operation" && operation is not null;

                //
                // Emitters for ALL unions (except Error)
                //
                if (!isError)
                {
                    string matchSource = MatchEmitter.Emit(union);
                    spc.AddSource($"{union.Name}.Match.g.cs", matchSource);
                }

                //
                // Emitters for Error union
                //
                if (isError)
                {
                    spc.AddSource($"{union.Name}.ErrorMatch.g.cs",
                        ErrorMatchEmitter.Emit(union));

                    spc.AddSource($"{union.Name}.ToString.g.cs",
                        ErrorToStringEmitter.Emit(union));
                }

                //
                // Emitters for Operation<T>
                //
                if (isOperation)
                {

                }
            }
        });
    }
}