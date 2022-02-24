using SerializIt.Generator.Model;
using System.Collections.Generic;

namespace SerializIt.Generator.Serializers;

internal abstract class BaseSerializer : ISerializer
{
    public virtual bool SkipNullValues => true;

    private readonly Dictionary<string, string> _strings = new();

    protected void AddString(string var, string value) => _strings[var] = value;

    public virtual void Strings(IndentedWriter writer)
    {
        foreach (var str in _strings)
        {
            writer.Write($@"private static readonly string {str.Key} = ""{str.Value}"";");
            writer.NewLine();
        }
    }

    public virtual void Usings(SerializeType typeInfo, IndentedWriter writer)
    {
        writer.Write("using System;");
        writer.NewLine();
        writer.Write("using System.Text;");
        writer.NewLine();
        writer.Write("using System.Linq;");
        writer.NewLine();
        writer.Write("using SerializIt;");
        writer.NewLine();
        writer.Write("using ");
        writer.Write(typeInfo.Namespace);
        writer.Write(";");
        writer.NewLine();
    }

    public virtual void StartElementFunc(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("[ThreadStatic] static IndentedWriter _writer;");
        writer.NewLine();
        writer.Write("internal void SerializeElement(");
        writer.Write(typeInfo.Namespace);
        writer.Write('.');
        writer.Write(typeInfo.TypeName);
        writer.Write(" item, IndentedWriter writer)");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
    }

    public virtual void EndElementFunc(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
    }

    public virtual void StartCode(IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("var writer = (_writer ??= new IndentedWriter());");
        writer.NewLine();
        writer.Write("try");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
    }

    public virtual void EndCode(IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("return writer.ToString();");
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
        writer.NewLine();
        writer.Write("finally");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
        writer.Write("writer.Clear();");
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
    }

    public virtual void StartNotDefaultCondition(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("if (");
        writer.Write(memberName);
        writer.Write(" != default)");
        writer.NewLine();
        writer.Write('{');
        writer.Indent++;
    }

    public virtual void EndNotDefaultCondition(string memberName, bool lastMember, IndentedWriter writer)
    {
        writer.Indent--;
        writer.NewLine();
        writer.Write('}');
    }

    public virtual void WriteValueMember(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write("writer.Write(");
        writer.Write(memberName);
        writer.Write(");");
    }

    public virtual void WriteStringMember(string memberName, IndentedWriter writer)
    {
        writer.NewLine();
        writer.Write(@"writer.Write('""')");
        writer.NewLine();
        writer.Write("writer.Write(");
        writer.Write(memberName);
        writer.Write(");");
        writer.NewLine();
        writer.Write(@"writer.Write('""');");
    }

    public virtual void WriteSerializedMember(string memberName, SerializeType serializedType, IndentedWriter writer)
    {
        writer.NewLine();
        if (memberName == null || serializedType == null)
        {
            writer.Write("SerializeElement(item");
            writer.Write(", writer);");
            return;
        }

        writer.Write("_context.");
        writer.Write(serializedType.TypeName);
        writer.Write(".SerializeElement(");
        writer.Write(memberName);
        writer.Write(", writer);");
    }

    public virtual void StartDocument(SerializeType typeInfo, IndentedWriter writer) { }
    public virtual void EndDocument(SerializeType typeInfo, IndentedWriter writer) { }

    public virtual void StartRootElement(SerializeType typeInfo, IndentedWriter writer) { }
    public virtual void EndRootElement(SerializeType typeInfo, IndentedWriter writer) { }

    public abstract void StartMember(SerializeMember member, bool firstMember, IndentedWriter writer);
    public abstract void EndMember(SerializeMember member, bool lastMember, IndentedWriter writer);

    public abstract string StartCollection(string typeName, string memberName, bool isArray, IndentedWriter writer);
    public abstract void EndCollection(string memberName, IndentedWriter writer);

    public void StartMemberNullCheck(string memberName, IndentedWriter sb)
    {
        AddString("_null", "null");
        sb.NewLine();
        sb.Write("if (");
        sb.Write(memberName);
        sb.Write(" == null)");
        sb.NewLine();
        sb.Write('{');
        sb.Indent++;
        sb.NewLine();
        sb.Write("writer.Write(_null);");
        sb.Indent--;
        sb.NewLine();
        sb.Write('}');
        sb.NewLine();
        sb.Write("else");
        sb.NewLine();
        sb.Write('{');
        sb.Indent++;
    }

    public void EndMemberNullCheck(string memberName, IndentedWriter sb)
    {
        sb.Indent--;
        sb.NewLine();
        sb.Write('}');
    }
}
