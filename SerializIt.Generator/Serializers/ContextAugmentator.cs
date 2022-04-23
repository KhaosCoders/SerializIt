using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SerializIt.Generator.Model;
using System;
using System.Text;

namespace SerializIt.Generator.Serializers;

internal static class ContextAugmentator
{
    public static void Augment(SerializationContext serializationContext, SourceProductionContext context)
    {
        IndentedWriter sb = new();

        sb.Write("using SerializIt;");
        sb.NewLine();
        sb.Write("using ");
        sb.Write(serializationContext.SerializerNamespace
                 ?? throw new ArgumentException($"{nameof(serializationContext.SerializerNamespace)} can't be null"));
        sb.Write(';');
        sb.NewLine();
        sb.Write("namespace ");
        sb.Write(serializationContext.ContextNamespace
                 ?? throw new ArgumentException($"{nameof(serializationContext.ContextNamespace)} can't be null"));
        sb.NewLine();
        sb.Write('{');
        sb.Indent++;
        sb.NewLine();
        sb.Write(serializationContext.Accessibility
                 ?? throw new ArgumentException($"{nameof(serializationContext.Accessibility)} can't be null"));
        sb.Write(" partial class ");
        sb.Write(serializationContext.ClassName
                 ?? throw new ArgumentException($"{nameof(serializationContext.ClassName)} can't be null"));
        sb.NewLine();
        sb.Write('{');
        sb.Indent++;
        sb.NewLine();

        if (serializationContext.SerializeTypes != null)
        {
            foreach (var typeInfo in serializationContext.SerializeTypes)
            {
                sb.Write("/// <summary>");
                sb.NewLine();
                sb.Write(@"/// Gets an instance of <see cref=""");
                sb.Write(typeInfo.SerializerName);
                sb.Write(@"""> to serialize a <see cref=""");
                sb.Write(typeInfo.TypeName
                         ?? throw new ArgumentException($"{nameof(typeInfo.TypeName)} can't be null"));
                sb.Write(@""">.");
                sb.NewLine();
                sb.Write("/// </summary>");
                sb.NewLine();
                sb.Write("public ");
                sb.Write(typeInfo.SerializerName);
                sb.Write(' ');
                sb.Write(typeInfo.TypeName);
                sb.Write(" => _");
                sb.Write(typeInfo.SerializerName);
                sb.Write(" ??= new(this);");
                sb.NewLine();
                sb.Write("private ");
                sb.Write(typeInfo.SerializerName);
                sb.Write(" _");
                sb.Write(typeInfo.SerializerName);
                sb.Write(';');
                sb.NewLine();
                sb.NewLine();
            }
        }

        sb.Indent--;
        sb.Write('}');
        sb.NewLine();
        sb.Indent--;
        sb.Write('}');
        sb.NewLine();

        var source = sb.ToString();
        context.AddSource($"{serializationContext.ContextNamespace}.{serializationContext.ClassName}.generated.cs", SourceText.From(source, Encoding.UTF8));
    }
}
