using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SerializIt.Generator.Model;
using System.Text;

namespace SerializIt.Generator.Serializers;

internal static class ContextAugmentator
{
    public static void Augment(SerializationContext serializationContext, GeneratorExecutionContext context)
    {
        ExtStringBuilder sb = new();

        sb.AppendLine("using SerializIt;")
          .Append("using ").Append(serializationContext.SerializerNamespace).AppendLine(";")
          .Append("namespace ").Append(serializationContext.ContextNamespace).AppendLine(" {")
          .IncreaseIndent()
          .Append(serializationContext.Accessability).Append(" partial class ").Append(serializationContext.ClassName).AppendLine(" {")
          .IncreaseIndent();

        foreach(var typeInfo in serializationContext.SerializeTypes)
        {
            sb.AppendLine("/// <summary>")
              .Append(@"/// Gets an instance of <see cref=""").Append(typeInfo.SerializerName).Append(@"""> to serialize a <see cref=""").Append(typeInfo.TypeName).AppendLine(@""">.")
              .AppendLine("/// </summary>")
              .Append("public ").Append(typeInfo.SerializerName).Append(' ').Append(typeInfo.TypeName)
              .Append(" => _").Append(typeInfo.SerializerName).AppendLine(" ??= new(this);")
              .Append("private ").Append(typeInfo.SerializerName).Append(" _").Append(typeInfo.SerializerName).AppendLine(";")
              .AppendLine("");
        }

        sb.DecreaseIndent()
          .AppendLine("}")
          .DecreaseIndent()
          .AppendLine("}");

        context.AddSource($"{serializationContext.ContextNamespace}.{serializationContext.ClassName}.generated.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}
