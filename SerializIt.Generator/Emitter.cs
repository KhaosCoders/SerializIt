using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SerializIt.Generator.Model;
using SerializIt.Generator.Serializers;

namespace SerializIt.Generator;

internal static class Emitter
{
    internal static void Emit(SourceProductionContext context,
        (Compilation Compilation, ImmutableArray<TypeDeclarationSyntax?> Types) source)
    {
        try
        {
#if LOGS
            Log.Debug("Emit {0} contexts: {1}{2}", source.Types.Length, Environment.NewLine, string.Join(Environment.NewLine, source.Types));
#endif
            var compilation = source.Compilation;

            foreach (var serializeContextClass in source.Types)
            {
                if (serializeContextClass == null)
                {
                    continue;
                }

                EmitSerializationContext(context, compilation, serializeContextClass);
            }
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to emit sources");
#endif
            throw;
        }
    }

    private static void EmitSerializationContext(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax serializeContextClass)
    {
        // Collect information about the serialization
        var serializationContext = Collector.CollectSerializationInfo(compilation, serializeContextClass);
        if (serializationContext == default)
        {
            return;
        }

        // Augment SerializationContext with serializer properties for each type
        var code = SerializationContextGenerator.Generate(serializationContext);
        context.AddSource($"{serializationContext.ContextNamespace}.{serializationContext.ClassName}.generated.cs", SourceText.From(code, Encoding.UTF8));

        // Build serializers
        foreach (var seralizeType in serializationContext.SerializeTypes)
        {
            EmitTypeSerializationClass(serializationContext, seralizeType, context);
        }
    }

    private static void EmitTypeSerializationClass(SerializationContext serializationContext, SerializeType serializationInfo, SourceProductionContext context)
    {
        var code = SerializationGenerator.Generate(serializationContext, serializationInfo);
        context.AddSource($"{serializationContext.SerializerNamespace}.{serializationInfo.SerializerName}.generated.cs", code);
    }
}
