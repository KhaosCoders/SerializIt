using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

[Serializer(ESerializers.Yaml)]
[SerializerOptions(typeof(YamlOptionsAttribute))]
internal class YamlSerializer : BaseSerializer
{
    public YamlSerializerOptions Options { get; set; }

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
        if (Options.AddPreamble)
        {
            sb.AppendLine(@".AppendLine(""---"")");
        }
    }

    public override void EndDocument(SerializeType typeInfo, ExtStringBuilder sb)
    {
        if (Options.AddPostamble)
        {
            sb.Append(@".NoIndent().AppendLine(""---"")");
        }
    }

    public override void StartRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        base.StartRootElement(typeInfo, sb);
        sb.AppendLine(".StartLayer()");
    }

    public override void EndRootElement(SerializeType typeInfo, ExtStringBuilder sb)
    {
        sb.AppendLine(".EndLayer()");
        base.EndRootElement(typeInfo, sb);
    }

    public override void StartMember(SerializeMember member, bool firstMember, ExtStringBuilder sb)
    {
        sb.Append(".SetLayer()")
          .Append(@".Append(""").Append(FormatMemberName(member.MemberName)).AppendLine(@": "")");
    }

    public override void EndMember(SerializeMember member, bool lastMember, ExtStringBuilder sb) { }

    public override void WriteSerializedMember(string memberName, SerializeType serializedType, ExtStringBuilder sb)
    {
        var isMember = memberName != null && serializedType != null;
        if (isMember)
        {
            sb.AppendLine(".IfLayer(sb => sb.AppendLine(string.Empty).IncreaseIndent())");
        }

        base.WriteSerializedMember(memberName, serializedType, sb);

        if (isMember)
        {
            sb.AppendLine(".IfLayer(sb => sb.DecreaseIndent())");
        }
    }

    public override string StartCollection(string typeName, string memberName, ExtStringBuilder sb)
    {
        sb.Append(".Conditional(() => !").Append(memberName).AppendLine(".Any(),").IncreaseIndent()
          .AppendLine(@"sb => sb.Append(""[ ""),")
          .AppendLine("sb => sb.AppendLine(string.Empty).IncreaseIndent()")
          .DecreaseIndent().AppendLine(")");

        sb.AppendLine(".Append(sb =>").IncreaseIndent()
          .AppendLine("{").IncreaseIndent()
          .Append("foreach(").Append(typeName).Append(" i in ").Append(memberName).AppendLine(")")
          .AppendLine("{").IncreaseIndent()
          .AppendLine(@"sb.Append(""- "").IncreaseIndent().StartLayer()");

        return "i";
    }

    public override void EndCollection(string memberName, ExtStringBuilder sb)
    {
        sb.AppendLine(".DecreaseIndent();")
          .DecreaseIndent().AppendLine("}")
          .DecreaseIndent().AppendLine("})")
          .DecreaseIndent();

        sb.Append(".Conditional(() => !").Append(memberName).AppendLine(".Any(),").IncreaseIndent()
          .AppendLine(@"sb => sb.Append(""]""),")
          .AppendLine("sb => sb.DecreaseIndent().EndLayer()")
          .DecreaseIndent().AppendLine(")");
    }

    public override void WriteStringMember(string memberName, ExtStringBuilder sb)
    {
        sb.Append(".Conditional(() => ").Append(memberName).Append(@".IndexOf('\n') >= 0,").IncreaseIndent()
          .Append(@"sb => sb.AppendLine(""|"").IncreaseIndent().AppendBlock(").Append(memberName).AppendLine(").DecreaseIndent(),")
          .Append("sb => sb.AppendLine(").Append(memberName).Append(")")
          .DecreaseIndent().AppendLine(")");
    }

    public override void WriteValueMember(string memberName, ExtStringBuilder sb)
    {
        sb.Append(".Append(").Append(memberName).AppendLine(").AppendLine(string.Empty)");
    }

    private string FormatMemberName(string name) =>
        char.ToLower(name[0]) + name.Substring(1);

}
