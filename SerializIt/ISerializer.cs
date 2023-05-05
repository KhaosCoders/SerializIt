using SerializIt.Generator.Model;

namespace SerializIt;

public interface ISerializer
{
    ISerializerOptions? Options { get; set; }

    bool SkipNullValues { get; }

    void Strings(IndentedWriter writer);

    void Usings(SerializeType typeInfo, IndentedWriter writer);

    void StartElementFunc(SerializationContext context, SerializeType typeInfo, IndentedWriter writer);
    void EndElementFunc(SerializationContext context, SerializeType typeInfo, IndentedWriter writer);

    void StartCode(IndentedWriter writer);
    void EndCode(IndentedWriter writer);

    void StartDocument(SerializeType typeInfo, IndentedWriter writer);
    void EndDocument(SerializeType typeInfo, IndentedWriter writer);

    void StartRootElement(SerializeType typeInfo, IndentedWriter writer);
    void EndRootElement(SerializeType typeInfo, IndentedWriter writer);

    void StartMember(SerializeMember member, bool firstMember, IndentedWriter writer);
    void EndMember(SerializeMember member, bool lastMember, IndentedWriter writer);

    string StartCollection(string typeName, string? memberName, bool isArray, EInline inline, IndentedWriter writer);
    void EndCollection(string? memberName, EInline inline, IndentedWriter writer);

    void WriteValueMember(string? memberName, EInline inline, IndentedWriter writer);
    void WriteStringMember(string? memberName, EInline inline, IndentedWriter writer);
    void WriteSerializedMember(string? memberName, SerializeType serializedType, EInline inline, IndentedWriter writer);

    void StartNotDefaultCondition(string memberName, IndentedWriter writer);
    void EndNotDefaultCondition(string memberName, bool lastMember, IndentedWriter writer);
    void StartMemberNullCheck(string memberName, IndentedWriter writer);
    void EndMemberNullCheck(string memberName, IndentedWriter writer);
    void Fields(SerializationContext context, SerializeType typeInfo, IndentedWriter writer);
}

