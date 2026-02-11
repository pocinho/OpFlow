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
            // Build semantic OperationModel (if present) – only for Operation<T>
            OperationModel? operation = OperationSemanticModelBuilder.TryBuild(unionList);

            if (operation is not null)
            {
                Diagnostic diag = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OPFQN",
                        title: "Operation FQN Debug",
                        messageFormat: "SuccessCaseFQN = '{0}', FailureCaseFQN = '{1}'",
                        category: "Debug",
                        DiagnosticSeverity.Info,
                        isEnabledByDefault: true),
                    Location.None,
                    operation.SuccessCaseFQN,
                    operation.FailureCaseFQN);

                spc.ReportDiagnostic(diag);
            }

            foreach (UnionModel union in unionList)
            {
                bool isError = union.Name == "Error";
                bool isOperation = union.Name == "Operation" && operation is not null;

                //
                // Emitters for Operation<T>
                //
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
                        new ToStringEmitter(),
                    ];

                    foreach (IOperationEmitter emitter in opEmitters)
                    {
                        spc.AddSource(emitter.FileName(operation!), emitter.Emit(operation!));
                    }
                }

                //
                // Emitters for Error union
                //
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