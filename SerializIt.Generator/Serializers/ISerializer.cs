using SerializIt.Generator.Model;

namespace SerializIt;

internal interface ISerializer
{
    void Usings(ExtStringBuilder sb, SerializeType typeInfo);

    void StartElementFunc(SerializationContext context, SerializeType typeInfo, ExtStringBuilder sb);
    void EndElementFunc(SerializationContext context, SerializeType typeInfo, ExtStringBuilder sb);

    void StartCode(ExtStringBuilder sb);
    void EndCode(ExtStringBuilder sb);

    void StartDocument(SerializeType typeInfo, ExtStringBuilder sb);
    void EndDocument(SerializeType typeInfo, ExtStringBuilder sb);

    void StartRootElement(SerializeType typeInfo, ExtStringBuilder sb);
    void EndRootElement(SerializeType typeInfo, ExtStringBuilder sb);

    void StartMember(SerializeMember member, bool firstMember, ExtStringBuilder sb);
    void EndMember(SerializeMember member, bool lastMember, ExtStringBuilder sb);

    string StartCollection(string typeName, string memberName, ExtStringBuilder sb);
    void EndCollection(string memberName, ExtStringBuilder sb);

    void WriteValueMember(string memberName, ExtStringBuilder sb);
    void WriteStringMember(string memberName, ExtStringBuilder sb);
    void WriteSerializedMember(string memberName, SerializeType serializedType, ExtStringBuilder sb);

    void StartNotDefaultCondition(string memberName, ExtStringBuilder sb);
    void EndNotDefaultCondition(string memberName, ExtStringBuilder sb);
}
