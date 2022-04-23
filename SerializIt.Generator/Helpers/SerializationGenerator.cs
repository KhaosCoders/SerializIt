﻿using System;
using System.Linq;
using System.Reflection;
using SerializIt.CodeAnalysis;
using SerializIt.Generator.Model;

namespace SerializIt.Generator.Helpers;

internal class SerializationGenerator
{
    private static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;

    public string GenerateSerializer(SerializationContext context, SerializeType typeInfo)
    {
        IndentedWriter sb = new();
        WriteSerializerClass(context, typeInfo, sb);
        return sb.ToString();
    }

    private static void WriteSerializerClass(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        writer.Write("// Generated by SerializIt SourceGenerator v.");
        writer.Write(version.ToString(3));
        writer.NewLine();
        writer.NewLine();

        context.Serializer?.Usings(typeInfo, writer);

        writer.Write("namespace ");
        writer.Write(context.SerializerNamespace
                     ?? throw new ArgumentException($"{nameof(context.SerializerNamespace)} can't be null"));
        writer.NewLine();
        writer.Write("{");
        writer.NewLine();
        writer.Indent++;
        writer.Write(context.Accessibility
                     ?? throw new ArgumentException($"{nameof(context.Accessibility)} can't be null"));
        writer.Write(" class ");
        writer.Write(typeInfo.SerializerName);
        writer.NewLine();
        writer.Write("{");
        writer.NewLine();
        writer.Indent++;
        writer.Write("private ");
        writer.Write(context.ContextNamespace
                     ?? throw new ArgumentException($"{nameof(context.ContextNamespace)} can't be null"));
        writer.Write('.');
        writer.Write(context.ClassName
                     ?? throw new ArgumentException($"{nameof(context.ClassName)} can't be null"));
        writer.Write(" _context;");
        writer.NewLine();
        writer.Write("public ");
        writer.Write(typeInfo.SerializerName);
        writer.Write("(");
        writer.Write(context.ContextNamespace);
        writer.Write('.');
        writer.Write(context.ClassName);
        writer.Write(" context)");
        writer.NewLine();
        writer.Write("{");
        writer.NewLine();
        writer.Indent++;
        writer.Write("_context = context;");
        writer.NewLine();
        writer.Indent--;
        writer.Write("}");
        writer.NewLine();

        context.Serializer?.StartElementFunc(context, typeInfo, writer);
        WriteRootElement(context, typeInfo, writer);
        context.Serializer?.EndElementFunc(context, typeInfo, writer);

        writer.NewLine();
        writer.Write(typeInfo.Accessibility);
        writer.Write(" string SerializeElement(");
        writer.Write(typeInfo.Namespace
                     ?? throw new ArgumentException($"{nameof(typeInfo.Namespace)} can't be null"));
        writer.Write('.');
        writer.Write(typeInfo.TypeName
                     ?? throw new ArgumentException($"{nameof(typeInfo.TypeName)} can't be null"));
        writer.Write(" item)");
        writer.NewLine();
        writer.Write("{");
        writer.Indent++;

        context.Serializer?.StartCode(writer);
        context.Serializer?.WriteSerializedMember(null, typeInfo, writer);
        context.Serializer?.EndCode(writer);

        writer.Indent--;
        writer.NewLine();
        writer.Write("}");
        writer.NewLine();
        writer.Write(typeInfo.Accessibility);
        writer.Write(" string SerializeDocument(");
        writer.Write(typeInfo.Namespace);
        writer.Write('.');
        writer.Write(typeInfo.TypeName);
        writer.Write(" item)");
        writer.NewLine();
        writer.Write("{");
        writer.Indent++;

        context.Serializer?.StartCode(writer);
        WriteDocument(context, typeInfo, writer);
        context.Serializer?.EndCode(writer);

        writer.Indent--;
        writer.NewLine();
        writer.Write("}");
        writer.NewLine();

        context.Serializer?.Strings(writer);

        writer.Indent--;
        writer.Write("}");
        writer.NewLine();
        writer.Indent--;
        writer.Write("}");
        writer.NewLine();
    }

    private static void WriteDocument(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        context.Serializer?.StartDocument(typeInfo, writer);
        context.Serializer?.WriteSerializedMember(null, typeInfo, writer);
        context.Serializer?.EndDocument(typeInfo, writer);
    }

    private static void WriteRootElement(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        context.Serializer?.StartRootElement(typeInfo, writer);
        WriteElementMembers(context, typeInfo, writer);
        context.Serializer?.EndRootElement(typeInfo, writer);
    }

    private static void WriteElementMembers(SerializationContext context, SerializeType typeInfo, IndentedWriter writer)
    {
        var members = typeInfo.Members.Where(m => !m.Skip).ToList();
        for (var i = 0; i < members.Count; i++)
        {
            var member = members[i];
            var memberName = $"item.{member.MemberName}";
            var firstMember = i == 0;
            var lastMember = i == members.Count - 1;
            if (member.SkipIfDefault)
            {
                context.Serializer?.StartNotDefaultCondition(memberName, writer);
            }
            context.Serializer?.StartMember(member, firstMember, writer);

            if (member.MemberType.IsReferenceType && context.Serializer?.SkipNullValues == false)
            {
                context.Serializer?.StartMemberNullCheck(memberName, writer);
            }

            WriteMemberValue(context, member.MemberType, memberName, writer);

            if (member.MemberType.IsReferenceType && context.Serializer?.SkipNullValues == false)
            {
                context.Serializer?.EndMemberNullCheck(memberName, writer);
            }

            context.Serializer?.EndMember(member, lastMember, writer);

            if (member.SkipIfDefault)
            {
                context.Serializer?.EndNotDefaultCondition(memberName, lastMember, writer);
            }
        }
    }

    private static void WriteMemberValue(SerializationContext context, TypeSymbol memberType, string? memberName, IndentedWriter writer)
    {
        if (memberType.Name == "String")
        {
            context.Serializer?.WriteStringMember(memberName, writer);
        }
        else if (memberType.IsCollection)
        {
            var typeSymbol = memberType.CollectionType;
            var typeName = $"{typeSymbol?.Namespace ?? throw new ArgumentException($"{nameof(typeSymbol.Namespace)} can't be null")}.{typeSymbol.Name}";
            var elementName = context.Serializer?.StartCollection(typeName, memberName, memberType.IsArray, writer);
            WriteMemberValue(context, typeSymbol, elementName, writer);
            context.Serializer?.EndCollection(memberName, writer);
        }
        else if (context.SerializeTypes?.FirstOrDefault(item => item.Type.CompareTo(memberType) == 0) is SerializeType serializedType)
        {
            context.Serializer?.WriteSerializedMember(memberName, serializedType, writer);
        }
        else if (memberType.IsValueType)
        {
            context.Serializer?.WriteValueMember(memberName, writer);
        }
    }

}
