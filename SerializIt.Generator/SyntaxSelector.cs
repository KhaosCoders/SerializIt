using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[assembly: InternalsVisibleTo("SerializIt.Tests")]

namespace SerializIt.Generator;

internal static class SyntaxSelector
{
    /// <summary>
    /// Decides which SyntaxNodes to inspect for a Serializer attribute
    /// </summary>
    internal static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        try
        {
            return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to check syntax node");
#endif
            throw;
        }
    }

    /// <summary>
    /// Selects only TypeDeclarationSyntax-Nodes with a Serializer attribute
    /// </summary>
    internal static TypeDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        try
        {
            var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;

            foreach (var attributeListSyntax in typeDeclarationSyntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol;
                    if (symbol is not IMethodSymbol attributeSymbol)
                    {
                        continue;
                    }

                    var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    var fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullName == Names.SerializerAttribute)
                    {
                        return typeDeclarationSyntax;
                    }
                }
            }

            return null;
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to get semantic targets");
#endif
            throw;
        }
    }
}
