using System;
using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

internal static class SerializationContextGenerator
{
    public static string Generate(SerializationContext serializationContext)
    {
        IndentedWriter writer = new();

        // Usings
        writer.Write("using SerializIt;");
        writer.NewLine();
        writer.Write("using ");
        writer.Write(serializationContext.SerializerNamespace
                 ?? throw new ArgumentException($"{nameof(serializationContext.SerializerNamespace)} can't be null"));
        writer.Write(';');
        writer.NewLine();

        // Namespace
        writer.Write("namespace ");
        writer.Write(serializationContext.ContextNamespace
                 ?? throw new ArgumentException($"{nameof(serializationContext.ContextNamespace)} can't be null"));
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();

        // SerializationContext class
        writer.Write(serializationContext.Accessibility
                 ?? throw new ArgumentException($"{nameof(serializationContext.Accessibility)} can't be null"));
        writer.Write(" partial class ");
        writer.Write(serializationContext.ClassName
                 ?? throw new ArgumentException($"{nameof(serializationContext.ClassName)} can't be null"));
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();

        // Serializer properties
        if (serializationContext.SerializeTypes != null)
        {
            WriteTypeProperties(serializationContext, writer);
        }

        // End class
        writer.Indent--;
        writer.Write('}');
        writer.NewLine();

        // End namespace
        writer.Indent--;
        writer.Write('}');
        writer.NewLine();

        return writer.ToString();
    }

    private static void WriteTypeProperties(SerializationContext serializationContext, IndentedWriter writer)
    {
        var count = serializationContext.SerializeTypes.Count;
        for (var i = 0; i < count; i++)
        {
            var typeInfo = serializationContext.SerializeTypes[i];
            // Xml comment
            writer.Write("/// <summary>");
            writer.NewLine();
            writer.Write(@"/// Gets an instance of <see cref=""");
            writer.Write(typeInfo.SerializerName);
            writer.Write(@"""> to serialize a <see cref=""");
            writer.Write(typeInfo.TypeName
                     ?? throw new ArgumentException($"{nameof(typeInfo.TypeName)} can't be null"));
            writer.Write(@""">.");
            writer.NewLine();
            writer.Write("/// </summary>");
            writer.NewLine();

            // Serializer property (lazily initialized)
            writer.Write("public ");
            writer.Write(typeInfo.SerializerName);
            writer.Write(' ');
            writer.Write(typeInfo.TypeName);
            writer.Write(" => _");
            writer.Write(typeInfo.SerializerName);
            writer.Write(" ??= new(this);");
            writer.NewLine();
            writer.Write("private ");
            writer.Write(typeInfo.SerializerName);
            writer.Write(" _");
            writer.Write(typeInfo.SerializerName);
            writer.Write(';');
            writer.NewLine();
            writer.NewLine();
        }
    }
}
