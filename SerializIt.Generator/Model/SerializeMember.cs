using Microsoft.CodeAnalysis;

namespace SerializIt.Generator.Model;
internal record SerializeMember
{
    public string MemberName { get; set; }

    public ITypeSymbol MemberType { get; set; }

    public int Order { get; set; }

    public bool Skip { get; set; }

    public bool SkipIfDefault { get; set; }

    public SerializeMember(string name, ITypeSymbol type)
    {
        MemberName = name;
        MemberType = type;
    }
}
