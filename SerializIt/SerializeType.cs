using SerializIt.CodeAnalysis;

namespace SerializIt.Generator.Model;

public record SerializeType
{
    public TypeSymbol Type { get; }
    public string? Namespace { get; }
    public string? TypeName { get; }

    public string SerializerName => $"{TypeName}Serializer";
    public string Accessibility { get; set; }

    public IList<SerializeMember> Members { get; set; } = new List<SerializeMember>();

    public SerializeType(TypeSymbol symbol)
    {
        Type = symbol ?? throw new ArgumentException($"Parameter {nameof(symbol)} can't be null");
        TypeName = symbol.Name;
        Namespace = symbol.Namespace;
        Accessibility = "public";
    }

    public int GetNextOrderIndex(int index = default)
    {
        if (index == default)
        {
            index = 0;
        }
        while (Members.Any(m => m.Order == index))
        {
            index++;
        }
        return index;
    }
}
