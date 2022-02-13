using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Xml)]
[SerializerOptions(typeof(XmlOptionsAttribute))]
internal class XmlSerializer : BaseSerializer
{
    public XmlSerializerOptions Options { get; set; }

    public override void StartCode(ExtStringBuilder sb)
    {
        base.StartCode(sb);
        if (Options.IndentChars != default)
        {
            sb.Append(@".SetIndentation(""").Append(Options.IndentChars).AppendLine(@""")");
        }
    }

    public override void StartDocument(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.AddDocTag)
        {
            sb.AppendLine(@".AppendLine(@""<?xml version=""""1.0"""" encoding=""""UTF-8""""?>"")");
        }
    }

    public override void StartRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.Append(@".AppendLine(""<").Append(FormatTagName(typeInfo.TypeName)).AppendLine(@">"").StartLayer().IncreaseIndent()");
        }
        else
        {
            sb.Append(@".AppendL(""<").Append(FormatTagName(typeInfo.TypeName)).AppendLine(@">"").StartLayer()");
        }
    }

    public override void EndRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.Append(@".DecreaseIndent().AppendLine(""</").Append(FormatTagName(typeInfo.TypeName)).AppendLine(@">"").EndLayer()");
        }
        else
        {
            sb.Append(@".Append(""</").Append(FormatTagName(typeInfo.TypeName)).AppendLine(@">"").EndLayer()");
        }
    }

    public override void StartMember(SerializeMember member, bool firstMember, ExtStringBuilder sb)
    {
        sb.Append(".SetLayer()");

        if (Options.PrettyPrint && !member.MemberType.IsValueType)
        {
            sb.Append(@".AppendLine(""<").Append(FormatTagName(member.MemberName)).AppendLine(@">"").IncreaseIndent()");
        }
        else
        {
            sb.Append(@".Append(""<").Append(FormatTagName(member.MemberName)).AppendLine(@">"")");
        }
    }

    public override void EndMember(SerializeMember member, bool lastMember, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.Append(@".DecreaseIndent().AppendLine(""</").Append(FormatTagName(member.MemberName)).AppendLine(@">"")");
        }
        else
        {
            sb.Append(@".Append(""</").Append(FormatTagName(member.MemberName)).AppendLine(@">"")");
        }
    }

    public override string StartCollection(string typeName, string memberName, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.Append(".Conditional(() => ").Append(memberName).AppendLine(".Any(),").IncreaseIndent()
              .AppendLine(@"sb => sb.IncreaseIndent()")
              .DecreaseIndent().AppendLine(")");
        }
        sb.AppendLine(".Append(sb =>").IncreaseIndent()
          .AppendLine("{").IncreaseIndent()
          .Append("foreach(").Append(typeName).Append(" i in ").Append(memberName).AppendLine(")")
          .AppendLine("{").IncreaseIndent()
          .Append("sb");

        return "i";
    }

    public override void EndCollection(string memberName, ExtStringBuilder sb)
    {
        sb.AppendLine(";")
          .DecreaseIndent().AppendLine("}")
          .DecreaseIndent().AppendLine("})")
          .DecreaseIndent();

        if (Options.PrettyPrint)
        {
            sb.Append(".Conditional(() => ").Append(memberName).AppendLine(".Any(),").IncreaseIndent()
              .AppendLine("sb => sb.DecreaseIndent()")
              .DecreaseIndent().AppendLine(")");
        }
    }

    public override void WriteStringMember(string memberName, ExtStringBuilder sb) =>
        sb.Append(@".Append(""<![CDATA["")")
          .Append(".Append(").Append(memberName).Append(")")
          .Append(@".Append(""]]>"")");

    private string FormatTagName(string name) =>
        Options.LowercaseTags ? name.ToLowerInvariant() : name;
}
