using System.Collections;
using System.Linq;
using Microsoft.CodeAnalysis;
using SerializIt.CodeAnalysis;
using SerializIt.Generator.Helpers;

namespace SerializIt.Generator;

internal static class TypeSymbolExtensions
{
    public static TypeSymbol ToTypeSymbol(this ITypeSymbol symbol)
    {
        var collectionType = symbol.AllInterfaces
            .Where(i => i.Name == nameof(IEnumerable) && i.TypeArguments.Length > 0)
            .Select(i => i.TypeArguments[0])
            .FirstOrDefault();
        var isCollection = collectionType != default;

        return new()
        {
            Name = symbol.Name,
            Namespace = SymbolHelper.GetNamespaceName(symbol.ContainingNamespace),
            IsValueType = symbol.IsValueType,
            IsReferenceType = symbol.IsReferenceType,
            IsArray = symbol.Kind == SymbolKind.ArrayType,
            IsCollection = isCollection,
            CollectionType = collectionType?.ToTypeSymbol()
        };
    }
}
