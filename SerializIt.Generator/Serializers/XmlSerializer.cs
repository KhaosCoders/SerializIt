using System;
using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Xml)]
[SerializerOptions(typeof(XmlOptionsAttribute))]
internal class XmlSerializer : BaseSerializer
{
    public XmlSerializerOptions? XmlOptions
    {
        get => Options is XmlSerializerOptions o ? o : null;
        set => Options = value;
    }

    public override void StartCode(IndentedWriter writer)
    {
        base.StartCode(writer);
        if (XmlOptions?.IndentChars is not string chars)
        {
            return;
        }
        writer.NewLine();
        writer.Write(@"writer.IndentChars = """);
        writer.Write(chars);
        writer.Write(@""";");
    }

    public override void StartDocument(SerializeType typeInfo, IndentedWriter writer)
    {
        if (XmlOptions?.AddDocTag != true)
        {
            return;
        }
        writer.NewLine();
        writer.Write(@"writer.Write(@""<?xml version=""""1.0"""" encoding=""""UTF-8""""?>"");");
        writer.NewLine();
        writer.Write("writer.NewLine();");
    }

    public override void StartRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write(@"writer.Write(""<");
        writer.Write(FormatTagName(typeInfo.TypeName
                                   ?? throw new ArgumentException($"{nameof(typeInfo.TypeName)} can't be null")));
        writer.Write(@">"");");
        writer.NewLine();
        writer.Write("writer.StartLayer();");
        if (XmlOptions?.PrettyPrint != true)
        {
            return;
        }
        writer.NewLine();
        writer.Write("writer.Indent++;");
        writer.NewLine();
        writer.Write("writer.NewLine();");
    }

    public override void EndRootElement(SerializeType typeInfo, IndentedWriter writer)
    {
        if (XmlOptions?.PrettyPrint == true)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
        }
        writer.NewLine();
        writer.Write(@"writer.Write(""</");
        writer.Write(FormatTagName(typeInfo.TypeName
                                   ?? throw new ArgumentException($"{nameof(typeInfo.TypeName)} can't be null")));
        writer.Write(@">"");");
        writer.NewLine();
        writer.Write("writer.EndLayer();");
    }

    public override void StartMember(SerializeMember member, EInline inline, bool firstMember, IndentedWriter writer)
    {
        if (member.MemberType.IsReferenceType)
        {
            writer.NewLine();
            writer.Write("if (item.");
            writer.Write(member.MemberName);
            writer.Write(" != null)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
        }

        writer.NewLine();
        writer.Write("writer.IsLayerSet = true;");
        writer.NewLine();
        writer.Write(@"writer.Write(""<");
        writer.Write(FormatTagName(member.MemberName));
        writer.Write(@">"");");

        if (XmlOptions?.PrettyPrint == true && !member.MemberType.IsValueType)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
            writer.NewLine();
            writer.Write("writer.Indent++;");
        }
    }

    public override void EndMember(SerializeMember member, bool lastMember, EInline inline, IndentedWriter writer)
    {
        if (XmlOptions?.PrettyPrint == true)
        {
            writer.NewLine();
            writer.Write("writer.Indent--;");
        }
        writer.NewLine();
        writer.Write(@"writer.Write(""</");
        writer.Write(FormatTagName(member.MemberName));
        writer.Write(@">"");");
        if (XmlOptions?.PrettyPrint == true)
        {
            writer.NewLine();
            writer.Write("writer.NewLine();");
        }

        if (!member.MemberType.IsReferenceType)
        {
            return;
        }
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
    }

    public override string StartCollection(string typeName, string? memberName, bool isArray, EInline inline, IndentedWriter writer)
    {
        if (XmlOptions?.PrettyPrint == true)
        {
            writer.NewLine();
            writer.Write("if(");
            writer.Write(memberName
                         ?? throw new ArgumentException($"{nameof(memberName)} can't be null"));
            writer.Write(isArray ? ".Length > 0)" : ".Count > 0)");
            writer.NewLine();
            writer.Write('{');
            writer.Indent++;
            writer.NewLine();
            writer.Write("writer.Indent++;");
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

        return $"{memberName}[n]";
    }

    public override void EndCollection(string? memberName, EInline inline, bool lastMember, IndentedWriter writer)
    {
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');

        if ((XmlOptions?.PrettyPrint) != true)
        {
            return;
        }
        writer.NewLine();
        writer.Write("writer.Indent--;");
        writer.Indent--;
        writer.NewLine();
        writer.Write("}");
    }

    public override void WriteStringMember(string? memberName, EInline inline, bool lastMember, IndentedWriter sb)
    {
        sb.NewLine();
        sb.Write(@"writer.Write(""<![CDATA["");");
        sb.NewLine();
        sb.Write("writer.Write(");
        sb.Write(memberName
                 ?? throw new ArgumentException($"{nameof(memberName)} can't be null"));
        sb.Write(");");
        sb.NewLine();
        sb.Write(@"writer.Write(""]]>"");");
    }

    private string FormatTagName(string name) =>
        XmlOptions?.LowercaseTags == true
            ? name.ToLowerInvariant()
            : name;
}
