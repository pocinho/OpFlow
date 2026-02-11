// Copyright (c) 2026 Paulo Pocinho.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpFlow.Generators.Emission;
using OpFlow.Generators.Emission.Emitters;
using OpFlow.Generators.Models;
using OpFlow.Generators.Parsing;
using OpFlow.Generators.Semantics;

namespace OpFlow.Generators.Generator;

[Generator]
public sealed class OperationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<UnionModel>> unions = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (ctx, _) => UnionParser.TryParse(ctx))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!)
            .Collect();

        context.RegisterSourceOutput(unions, static (spc, unionList) =>
        {
            OperationModel? operation = OperationSemanticModelBuilder.TryBuild(unionList);

            foreach (UnionModel union in unionList)
            {
                bool isError = union.Name == "Error";
                bool isOperation = union.Name == "Operation" && operation is not null;

                if (isOperation)
                {
                    IOperationEmitter[] opEmitters =
                    [
                        // Creation
                        new FromEmitter(),
                        new FailEmitter(),
                        new TryEmitter(),

                        // Transform
                        new MapEmitter(),
                        new BindEmitter(),
                        new MapErrorEmitter(),
                        new BindErrorEmitter(),

                        // Guards
                        new EnsureEmitter(),
                        new ValidateEmitter(),
                        new RecoverEmitter(),

                        // Side Effects
                        new TapEmitter(),
                        new TapErrorEmitter(),
                        new FinallyEmitter(),

                        // Control Flow
                        new IfSuccessEmitter(),
                        new IfFailureEmitter(),
                        new MatchEmitter(),
                        new SwitchEmitter(),
                        new FlattenEmitter(),

                        // Representation
                        //new ToStringEmitter(),
                    ];

                    foreach (IOperationEmitter emitter in opEmitters)
                    {
                        spc.AddSource(emitter.FileName(operation!), emitter.Emit(operation!));
                    }
                }

                if (isError)
                {
                    IErrorEmitter[] errorEmitters =
                    [
                        new ErrorToStringEmitter(),
                    ];

                    foreach (IErrorEmitter emitter in errorEmitters)
                    {
                        spc.AddSource(emitter.FileName(union), emitter.Emit(union));
                    }
                }
            }
        });
    }
}