using SerializIt.CodeAnalysis;

namespace SerializIt.Generator.Model;

public record SerializeMember
{
    public string MemberName { get; set; }

    public TypeSymbol MemberType { get; set; }

    public int Order { get; set; }

    public bool Skip { get; set; }

    public bool SkipIfDefault { get; set; }

    public SerializeMember(string name, TypeSymbol type)
    {
        MemberName = name;
        MemberType = type;
    }
}
