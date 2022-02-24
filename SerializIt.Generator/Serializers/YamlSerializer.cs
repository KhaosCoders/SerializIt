using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Yaml)]
[SerializerOptions(typeof(YamlOptionsAttribute))]
internal class YamlSerializer : BaseSerializer
{
    public YamlSerializerOptions Options { get; set; }

    public override bool SkipNullValues => false;

    public override void StartCode(IndentedWriter writer)
    {
        base.StartCode(writer);
        if (Options.IndentChars != default)
        {
            writer.NewLine();
            writer.Write(@"writer.IndentChars = """);
            writer.Write(Options.IndentChars);
            writer.Write(@""";");
        }
    }

    public override void StartDocument(SerializeType typeInfo, IndentedWriter writer)
    {
        if (Options.AddPreamble)
        {
            writer.NewLine();
            writer.Write(@"writer.Write(""---"");");
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }
    }

    public override void EndDocument(SerializeType typeInfo, IndentedWriter writer)
    {
        if (Options.AddPostamble)
        {
            writer.NewLine();
            writer.Write("writer.NoIndent();");
            writer.NewLine();
            writer.Write(@"writer.Write(""---"");");
        }
    }

    public override void StartRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        base.StartRootElement(typeInfo, writer);
        writer.NewLine();
        writer.Write("writer.StartLayer();");
    }

    public override void EndRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.EndLayer();");
        base.EndRootElement(typeInfo, writer);
    }

    public override void StartMember(SerializeMember member, bool firstMember, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.IsLayerSet = true;");
        writer.NewLine();
        writer.Write(@"writer.Write(""");
        writer.Write(FormatMemberName(member.MemberName));
        writer.Write(@": "");");
    }

    public override void EndMember(SerializeMember member, bool lastMember, IndentedWriter writer) { }

    public override void WriteSerializedMember(string memberName, SerializeType serializedType, IndentedWriter writer)
    {
        var isMember = memberName != null && serializedType != null;
        if (isMember)
        {
            writer.NewLine();
            writer.Write("if (writer.IsLayerSet)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.NewLine();
            writer.Write("writer.Indent++;");
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
        }

        base.WriteSerializedMember(memberName, serializedType, writer);

        if (isMember)
        {
            writer.NewLine();
            writer.Write("if (writer.IsLayerSet)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.Indent--;");
            writer.Indent--;
            writer.NewLine();
            writer.Write('}');
        }
    }

    public override string StartCollection(string typeName, string memberName, bool isArray, IndentedWriter writer)
    {
        writer.Write("if (");
        writer.Write(memberName);
        if (isArray)
        {
            writer.Write(".Length > 0)");
        }
        else
        {
            writer.Write(".Count > 0)");
        }
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();
        writer.Write(@"writer.Write(""[ ]"");");
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
        writer.NewLine();
        writer.Write("else");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();
        writer.Write("writer.NewLine();");
        writer.NewLine();
        writer.Write("writer.Indent++;");
        writer.NewLine();

        writer.Write("foreach(");
        writer.Write(typeName);
        writer.Write(" x in ");
        writer.Write(memberName);
        writer.Write(")");
        writer.NewLine();
        writer.Write("{");
        writer.Indent++;
        writer.NewLine();
        writer.Write(@"writer.Write(""- "");");
        writer.NewLine();
        writer.Write("writer.Indent++;");
        writer.NewLine();
        writer.Write("writer.StartLayer();");

        return "x";
    }

    public override void EndCollection(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.EndLayer();");
        writer.NewLine();
        writer.Write("writer.Indent--;");
        writer.Indent--;
        writer.NewLine();
        writer.Write("}");
        writer.Indent--;
        writer.NewLine();
        writer.Write("}");
    }

    public override void WriteStringMember(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("if (");
        writer.Write(memberName);
        writer.Write(@".IndexOf('\n') >= 0)");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();
        writer.Write("writer.Write('|');");
        writer.NewLine();
        writer.Write("writer.Indent++;");
        writer.NewLine();
        writer.Write("writer.NewLine();");
        writer.NewLine();
        writer.Write("writer.WriteBlock(");
        writer.Write(memberName);
        writer.Write(");");
        writer.NewLine();
        writer.Write("writer.Indent--;");
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
        writer.NewLine();
        writer.Write("else");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.NewLine();
        writer.Write("writer.Write(");
        writer.Write(memberName);
        writer.Write(");");
        writer.NewLine();
        writer.Write("writer.NewLine();");
        writer.Indent--;
        writer.Write('}');
    }

    public override void WriteValueMember(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.Write(");
        writer.Write(memberName);
        writer.Write(");");
        writer.NewLine();
        writer.Write("writer.NewLine();");
    }

    private string FormatMemberName(string name) =>
        char.ToLower(name[0]) + name.Substring(1);

}
