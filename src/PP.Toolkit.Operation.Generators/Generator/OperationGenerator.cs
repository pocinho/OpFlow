// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PP.Toolkit.Operation.Generators.Emission;
using PP.Toolkit.Operation.Generators.Emission.Emitters;
using PP.Toolkit.Operation.Generators.Models;
using PP.Toolkit.Operation.Generators.Parsing;
using PP.Toolkit.Operation.Generators.Semantics;

namespace PP.Toolkit.Operation.Generators.Generator;

[Generator]
public sealed class OperationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Parse all unions
        IncrementalValueProvider<ImmutableArray<UnionModel>> unions = context.SyntaxProvider
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

            // Emitters for generic unions
            foreach (UnionModel? union in unionList)
            {
                bool isError = union.Name == "Error";
                bool isOperation = union.Name == "Operation" && operation is not null;

                //
                // Emitters for Operation<T>
                //
                if (isOperation)
                {
                    IOperationEmitter[] emitters =
                    {
                        // Canon:
                        //
                        // Creation:
                        new FromEmitter(),
                        new FailEmitter(),
                        new TryEmitter(),
                        // Transform:
                        new MapEmitter(),
                        new BindEmitter(),
                        new MapErrorEmitter(),
                        new BindErrorEmitter(),
                        // Guards:
                        new EnsureEmitter(),
                        new ValidateEmitter(),
                        new RecoverEmitter(),
                    };

                    foreach (IOperationEmitter emitter in emitters)
                    {
                        spc.AddSource(emitter.FileName(operation!), emitter.Emit(operation!));
                    }
                }
            }
        });
    }
}