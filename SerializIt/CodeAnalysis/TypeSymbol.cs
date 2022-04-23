
namespace SerializIt.CodeAnalysis;

public record TypeSymbol : IComparable
{
    public string? Name { get; set; }
    public string? Namespace { get; set; }
    public bool IsReferenceType { get; set; }
    public bool IsValueType { get; set; }
    public bool IsArray { get; set; }
    public bool IsCollection { get; set; }
    public TypeSymbol? CollectionType { get; set; }

    public int CompareTo(object obj)
    {
        if (obj is not TypeSymbol symbol)
        {
            return -1;
        }
        return $"{Namespace}.{Name}".CompareTo($"{symbol.Namespace}.{symbol.Name}");
    }
}
