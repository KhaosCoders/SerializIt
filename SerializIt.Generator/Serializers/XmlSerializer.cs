using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Xml)]
[SerializerOptions(typeof(XmlOptionsAttribute))]
internal class XmlSerializer : BaseSerializer
{
    public XmlSerializerOptions Options { get; set; }

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
        if (Options.AddDocTag)
        {
            writer.NewLine();
            writer.Write(@"writer.Write(@""<?xml version=""""1.0"""" encoding=""""UTF-8""""?>"");");
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }
    }

    public override void StartRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write(@"writer.Write(""<");
        writer.Write(FormatTagName(typeInfo.TypeName));
        writer.Write(@">"");");
        writer.NewLine();
        writer.Write("writer.StartLayer();");
        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.Indent++;");
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }
    }

    public override void EndRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
        }
        writer.NewLine();
        writer.Write(@"writer.Write(""</");
        writer.Write(FormatTagName(typeInfo.TypeName));
        writer.Write(@">"");");
        writer.NewLine();
        writer.Write("writer.EndLayer();");
    }

    public override void StartMember(SerializeMember member, bool firstMember, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.IsLayerSet = true;");
        writer.NewLine();
        writer.Write(@"writer.Write(""<");
        writer.Write(FormatTagName(member.MemberName));
        writer.Write(@">"");");

        if (Options.PrettyPrint && !member.MemberType.IsValueType)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.NewLine();
            writer.Write("writer.Indent++;");
        }
    }

    public override void EndMember(SerializeMember member, bool lastMember, IndentedWriter writer)
    {
        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
        }
        writer.NewLine();
        writer.Write(@"writer.Write(""</");
        writer.Write(FormatTagName(member.MemberName));
        writer.Write(@">"");");
        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }
    }

    public override string StartCollection(string typeName, string memberName, bool isArray, IndentedWriter writer)
    {
        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("if(");
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
            writer.Write("writer.Indent++;");
        }
        writer.NewLine();
        writer.Write("for(int n = 0;n < ");
        writer.Write(memberName);
        if (isArray)
        {
            writer.Write(".Length");
        }
        else
        {
            writer.Write(".Count");
        }
        writer.Write(";n++)");
        writer.NewLine();
        writer.Write("{");
        writer.Indent++;

        return $"{memberName}[n]";
    }

    public override void EndCollection(string memberName, IndentedWriter writer)
    {
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');

        if (Options.PrettyPrint)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
            writer.Indent--;
            writer.NewLine();
            writer.Write("}");
        }
    }

    public override void WriteStringMember(string memberName, IndentedWriter sb)
    {
        sb.NewLine();
        sb.Write(@"writer.Write(""<![CDATA["");");
        sb.NewLine();
        sb.Write("writer.Write(");
        sb.Write(memberName);
        sb.Write(");");
        sb.NewLine();
        sb.Write(@"writer.Write(""]]>"");");
    }

    private string FormatTagName(string name) =>
        Options.LowercaseTags ? name.ToLowerInvariant() : name;
}
