// Copyright (c) 2026 Paulo Pocinho.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PP.Toolkit.Operation.Generators.Emission.Emitters;
using PP.Toolkit.Operation.Generators.Models;
using PP.Toolkit.Operation.Generators.Parsing;

namespace PP.Toolkit.Operation.Generators.Generator;

[Generator]
public sealed class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        System.Diagnostics.Debug.WriteLine("UnionGenerator running");

        // 1. Pick only record declarations as candidates
        IncrementalValuesProvider<UnionModel?> candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (ctx, _) => UnionParser.TryParse(ctx))
            .Where(static model => model is not null);

        // 2. Emit code for each parsed union
        context.RegisterSourceOutput(candidates, static (spc, modelObj) =>
        {
            UnionModel model = modelObj!;

            bool isError = model.Name == "Error";
            bool isOperation = model.Name == "Operation" && model.GenericParameters.Count() == 1;

            //
            // Emitters for ALL unions except Error
            //
            if (!isError)
            {
                string matchSource = MatchEmitter.Emit(model);
                spc.AddSource($"{model.Name}.Match.g.cs", matchSource);
            }

            //
            // Emitters for Error union
            //
            if (isError)
            {
                string errorMatchSource = ErrorMatchEmitter.Emit(model);
                spc.AddSource($"{model.Name}.ErrorMatch.g.cs", errorMatchSource);

                string errorToStringSource = ErrorToStringEmitter.Emit(model);
                spc.AddSource($"{model.Name}.ToString.g.cs", errorToStringSource);
            }

            //
            // Emitters for Operation<T>
            //
            if (isOperation)
            {
                spc.AddSource($"{model.Name}.Bind.g.cs", OperationBindEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Combine.g.cs", OperationCombineEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Ensure.g.cs", OperationEnsureEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Fallback.g.cs", OperationFallbackEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Finally.g.cs", OperationFinallyEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Flatten.g.cs", OperationFlattenEmitter.Emit(model));
                spc.AddSource($"{model.Name}.If.g.cs", OperationIfEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Linq.g.cs", OperationLinqEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Map.g.cs", OperationMapEmitter.Emit(model));
                spc.AddSource($"{model.Name}.MapError.g.cs", OperationMapErrorEmitter.Emit(model));
                spc.AddSource($"{model.Name}.OrElse.g.cs", OperationOrElseEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Recover.g.cs", OperationRecoverEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Tap.g.cs", TapEmitter.Emit(model));
                spc.AddSource($"{model.Name}.ToString.g.cs", OperationToStringEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Where.g.cs", OperationWhereEmitter.Emit(model));
                spc.AddSource($"{model.Name}.Zip.g.cs", OperationZipEmitter.Emit(model));
            }
        });
    }
}