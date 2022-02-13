using Microsoft.CodeAnalysis;
using SerializIt.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerializIt.Generator.Model;

internal record SerializeType
{
    public ITypeSymbol Type { get; }
    public string Namespace { get; }
    public string TypeName { get; }

    public string SerializerName => $"{TypeName}Serializer";
    public string Accessability { get; set; }

    public IList<SerializeMember> Members { get; set; } = new List<SerializeMember>();

    public SerializeType(ITypeSymbol symbol)
    {
        Type = symbol;
        TypeName = symbol.Name;
        Namespace = SymbolHelper.GetNamespaceName(symbol.ContainingNamespace);
        Accessability = "public";
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
