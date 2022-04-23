using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace SerializIt.Generator.Helpers;

internal static class SymbolHelper
{
    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol? symbol, Accessibility accessibility, SymbolKind kind)
    {
        if (symbol == null)
        {
            yield break;
        }

        foreach (var member in symbol.GetMembers().Where(m => m.DeclaredAccessibility == accessibility && m.Kind == kind))
        {
            yield return member;
        }

        if (symbol.BaseType is INamedTypeSymbol baseType)
        {
            foreach (var member in baseType.GetAllMembers(accessibility, kind))
            {
                yield return member;
            }
        }
    }

    public static string? GetNamespaceName(INamespaceSymbol? symbol)
    {
        if (string.IsNullOrEmpty(symbol?.Name))
        {
            return null;
        }

        string? ns = null;
        if (symbol?.ContainingNamespace is INamespaceSymbol parentNs)
        {
            ns = GetNamespaceName(parentNs);
        }
        return ns != null ? $"{ns}.{symbol?.Name}" : symbol?.Name;
    }

    public static string GetAccessor(Accessibility accessibility) =>
         accessibility == Accessibility.Public ? "public" : "internal";
}
