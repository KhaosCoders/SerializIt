using System;
using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Json)]
[SerializerOptions(typeof(JsonOptionsAttribute))]
internal class JsonSerializer : BaseSerializer
{
    public JsonSerializerOptions JsonOptions
    {
        get => Options is JsonSerializerOptions o ? o : default;
        set => Options = value;
    }

    public override bool SkipNullValues => JsonOptions.SkipNullValues;

    public override void Usings(SerializeType typeInfo, IndentedWriter writer)
    {
        base.Usings(typeInfo, writer);
        writer.Write("using System.Web;");
        writer.NewLine();
    }

    public override void StartCode(IndentedWriter writer)
    {
        base.StartCode(writer);
        if (JsonOptions.IndentChars != default)
        {
            writer.NewLine();
            writer.Write(@"writer.IndentChars = """);
            writer.Write(JsonOptions.IndentChars);
            writer.Write(@""";");
        }
    }

    public override void StartRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.Write('{');");
        writer.NewLine();
        writer.Write("writer.StartLayer();");
        if (JsonOptions.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.NewLine();
            writer.Write("writer.Indent++;");
        }
    }

    public override void EndRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        if (JsonOptions.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("if (writer.IsLayerSet)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
            writer.NewLine();
            writer.Write("writer.Indent--;");
        }
        writer.NewLine();
        writer.Write("writer.Write('}');");
        writer.NewLine();
        writer.Write("writer.EndLayer();");
    }

    public override void StartMember(SerializeMember member, bool firstMember, IndentedWriter writer)
    {
        if (member.MemberType.IsReferenceType && JsonOptions.SkipNullValues)
        {
            writer.NewLine();
            writer.Write("if (item.");
            writer.Write(member.MemberName);
            writer.Write(" != null)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
        }
        if (!firstMember)
        {
            writer.NewLine();
            writer.Write("if (writer.IsLayerSet)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.Write(',');");
            if (JsonOptions.PrettyPrint)
            {
                writer.NewLine();
                writer.Write("writer.NewLine();");
            }
            writer.Indent--;
            writer.NewLine();
            writer.Write("}");
        }
        writer.NewLine();
        writer.Write("writer.IsLayerSet = true;");
        writer.NewLine();
        AddString($"_prop{member.MemberName}",
            @$"\{JsonOptions.Quotes}{member.MemberName}\{JsonOptions.Quotes}{(JsonOptions.PrettyPrint ? ": " : ":")}");
        writer.Write($"writer.Write(_prop{member.MemberName});");
    }

    public override void EndMember(SerializeMember member, bool lastMember, IndentedWriter writer)
    {
        if (member.MemberType.IsReferenceType && JsonOptions.SkipNullValues)
        {
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
        }
    }

    public override string StartCollection(string typeName, string? memberName, bool isArray, IndentedWriter writer)
    {
        writer.NewLine();
        if (JsonOptions.PrettyPrint)
        {
            writer.Write("if (");
            writer.Write(memberName
                         ?? throw new ArgumentException($"{nameof(memberName)} can't be null"));
            writer.Write(isArray ? ".Length == 0)" : ".Count == 0)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            AddString("_emptyArray", "[]");
            writer.Write("writer.Write(_emptyArray);");
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
            writer.NewLine();
            writer.Write("else");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.Write('[');");
            writer.NewLine();
            writer.Write("writer.Indent++;");
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }
        else
        {
            writer.Write("writer.Write('[');");
        }

        writer.NewLine();
        writer.Write("for(int n = 0;n < ");
        writer.Write(memberName
                     ?? throw new ArgumentException($"{nameof(memberName)} can't be null"));
        writer.Write(isArray ? ".Length" : ".Count");
        writer.Write(";n++)");
        writer.NewLine();
        writer.Write("{");
        writer.Indent++;
        writer.NewLine();
        writer.Write("if (n > 0)");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();

        writer.Write("writer.Write(',');");
        if (JsonOptions.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }

        writer.Indent--;
        writer.NewLine();
        writer.Write('}');

        return $"{memberName}[n]";
    }

    public override void EndCollection(string? memberName, IndentedWriter writer)
    {
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');

        if (JsonOptions.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.NewLine();
            writer.Write("writer.Write(']');");
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
        }
        else
        {
            writer.NewLine();
            writer.Write("writer.Write(']');");
        }
    }

    public override void WriteStringMember(string? memberName, IndentedWriter writer)
    {
        writer.NewLine();
        AddString("_quotes",@$"\{JsonOptions.Quotes}");
        writer.Write("writer.Write(_quotes);");
        writer.NewLine();
        writer.Write("writer.Write(HttpUtility.JavaScriptStringEncode(");
        writer.Write(memberName
                     ?? throw new ArgumentException($"{nameof(memberName)} can't be null"));
        writer.Write("));");
        writer.NewLine();
        writer.Write("writer.Write(_quotes);");
    }

    private static string FormatMemberName(string name) =>
        string.IsNullOrEmpty(name)
            ? name
            : char.ToLower(name[0]) + name.Substring(1);
}
