using SerializIt.Generator.Model;

namespace SerializIt.Generator.Serializers;

internal abstract class BaseSerializer : ISerializer
{
    public virtual void Usings(ExtStringBuilder sb, SerializeType typeInfo) =>
    sb.AppendLine("using System.Text;")
      .AppendLine("using System.Linq;")
      .AppendLine("using SerializIt;")
      .Append("using ").Append(typeInfo.Namespace).AppendLine(";");

    public virtual void StartElementFunc(SerializationContext context, SerializeType typeInfo, ExtStringBuilder sb) =>
        sb.Append("internal void SerializeElement(").Append(typeInfo.Namespace).Append('.').Append(typeInfo.TypeName).AppendLine(" item, ExtStringBuilder sb)")
          .AppendLine("{").IncreaseIndent()
          .AppendLine("sb").IncreaseIndent();
    public virtual void EndElementFunc(SerializationContext context, SerializeType typeInfo, ExtStringBuilder sb) =>
        sb.AppendLine(";").DecreaseIndent()
          .DecreaseIndent().AppendLine("}");

    public virtual void StartCode(ExtStringBuilder sb) =>
        sb.AppendLine("ExtStringBuilder sb = new ExtStringBuilder();")
          .AppendLine("sb").IncreaseIndent();
    public virtual void EndCode(ExtStringBuilder sb) =>
        sb.AppendLine(";").DecreaseIndent()
          .AppendLine("return sb.ToString();");

    public virtual void StartNotDefaultCondition(string memberName, ExtStringBuilder sb) =>
        sb.Append(".Conditional(() => ").Append(memberName).AppendLine(" != default,").IncreaseIndent()
          .AppendLine("sb => sb");

    public virtual void EndNotDefaultCondition(string memberName, ExtStringBuilder sb) =>
        sb.DecreaseIndent().AppendLine(")");

    public virtual void WriteValueMember(string memberName, ExtStringBuilder sb) =>
        sb.Append(".Append(").Append(memberName).AppendLine(")");

    public virtual void WriteStringMember(string memberName, ExtStringBuilder sb) =>
        sb.Append(@".Append(""\"""")")
          .Append(".Append(").Append(memberName).Append(")")
          .Append(@".Append(""\"""")");

    public virtual void WriteSerializedMember(string memberName, SerializeType serializedType, ExtStringBuilder sb)
    {
        if (memberName == null || serializedType == null)
        {
            sb.Append(".Append(sb => this.SerializeElement(item").AppendLine(", sb))");
            return;
        }

        sb.Append(".Append(sb => _context.").Append(serializedType.TypeName).Append(".SerializeElement(").Append(memberName).AppendLine(", sb))");
    }

    public virtual void StartDocument(SerializeType typeInfo, ExtStringBuilder sb) { }
    public virtual void EndDocument(SerializeType typeInfo, ExtStringBuilder sb) { }

    public virtual void StartRootElement(SerializeType typeInfo, ExtStringBuilder sb) { }
    public virtual void EndRootElement(SerializeType typeInfo, ExtStringBuilder sb) { }

    public abstract void StartMember(SerializeMember member, bool firstMember, ExtStringBuilder sb);
    public abstract void EndMember(SerializeMember member, bool lastMember, ExtStringBuilder sb);

    public abstract string StartCollection(string typeName, string memberName, ExtStringBuilder sb);
    public abstract void EndCollection(string memberName, ExtStringBuilder sb);
}
