using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Json)]
[SerializerOptions(typeof(JsonOptionsAttribute))]
internal class JsonSerializer : BaseSerializer
{
    public JsonSerializerOptions Options { get; set; }

    public override bool SkipNullValues => Options.SkipNullValues;

    public override void Usings(ExtStringBuilder sb, SerializeType typeInfo)
    {
        base.Usings(sb, typeInfo);
        sb.AppendLine("using System.Web;");
    }

    public override void StartCode(ExtStringBuilder sb)
    {
        base.StartCode(sb);
        if (Options.IndentChars != default)
        {
            sb.Append(@".SetIndentation(""").Append(Options.IndentChars).AppendLine(@""")");
        }
    }

    public override void EndNotDefaultCondition(string memberName, ExtStringBuilder sb) =>
        sb.AppendLine(",")
          .AppendLine("sb => sb.IfLayer(sb => sb.AppendLine(string.Empty))")
          .DecreaseIndent().AppendLine(")");

    public override void StartRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.AppendLine(@".AppendLine(""{"").StartLayer().IncreaseIndent()");
        }
        else
        {
            sb.AppendLine(@".Append(""{"").StartLayer()");
        }
    }

    public override void EndRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.AppendLine(@".DecreaseIndent().Append(""}"").EndLayer()");
        }
        else
        {
            sb.AppendLine(@".Append(""}"").EndLayer()");
        }
    }

    public override void StartMember(SerializeMember member, bool firstMember, ExtStringBuilder sb)
    {
        if (Options.SkipNullValues)
        {
            sb.Append(".Conditional(() => item.").Append(member.MemberName).AppendLine(" != null,").IncreaseIndent()
              .Append("sb => sb");
        }
        if (!firstMember)
        {
            if (Options.PrettyPrint)
            {
                sb.AppendLine(@".IfLayer(sb => sb.AppendLine("",""))");
            }
            else
            {
                sb.AppendLine(@".IfLayer(sb => sb.Append("",""))");
            }
        }
        sb.Append(".SetLayer()")
          .Append(@".Append(""\").Append(Options.Quotes).Append(FormatMemberName(member.MemberName)).Append(@"\").Append(Options.Quotes).Append(@""")");
        if (Options.PrettyPrint)
        {
            sb.AppendLine(@".Append("": "")");
        }
        else
        {
            sb.AppendLine(".Append(':')");
        }
    }

    public override void EndMember(SerializeMember member, bool lastMember, ExtStringBuilder sb)
    {
        if (lastMember && Options.PrettyPrint)
        {
            sb.AppendLine(".AppendLine(string.Empty)");
        }
        if (Options.SkipNullValues)
        {
            sb.DecreaseIndent().AppendLine(")");
        }
    }

    public override string StartCollection(string typeName, string memberName, ExtStringBuilder sb)
    {
        if (Options.PrettyPrint)
        {
            sb.Append(".Conditional(() => ").Append(memberName).AppendLine(".Any(),").IncreaseIndent()
              .AppendLine(@"sb => sb.AppendLine(""["").IncreaseIndent(),")
              .AppendLine(@"sb => sb.Append(""["")")
              .DecreaseIndent().AppendLine(")");
        }
        else
        {
            sb.AppendLine(@".Append(""["")");
        }
        sb.AppendLine(".Append(sb =>").IncreaseIndent()
          .AppendLine("{").IncreaseIndent()
          .AppendLine("bool first = true;")
          .Append("foreach(").Append(typeName).Append(" i in ").Append(memberName).AppendLine(")")
          .AppendLine("{").IncreaseIndent()
          .AppendLine("if (!first)").IncreaseIndent();

        if (Options.PrettyPrint)
        {
            sb.AppendLine(@"sb.AppendLine("","");");
        }
        else
        {
            sb.AppendLine(@"sb.Append("","");");
        }

        sb.DecreaseIndent()
          .AppendLine("first = false;")
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
              .AppendLine(@"sb => sb.DecreaseIndent().AppendLine(string.Empty).Append(""]""),")
              .AppendLine(@"sb => sb.Append(""]"")")
              .DecreaseIndent().AppendLine(")");
        }
        else
        {
            sb.AppendLine(@".Append(""]"")");
        }
    }

    public override void WriteStringMember(string memberName, ExtStringBuilder sb) =>
        sb.Append(@".Append(""\").Append(Options.Quotes).Append(@""")")
          .Append(".Append(HttpUtility.JavaScriptStringEncode(").Append(memberName).Append("))")
          .Append(@".Append(""\").Append(Options.Quotes).Append(@""")");

    private string FormatMemberName(string name) =>
        char.ToLower(name[0]) + name.Substring(1);
}
