using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SerializIt.Generator.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SerializIt.Generator;

internal class SyntaxReceiver : ISyntaxReceiver
{
    private const string fullAttrName = nameof(Resources.SerializeTypeAttribute);
    private static readonly string shortAttrName = fullAttrName.Substring(0, fullAttrName.Length - "Attribute".Length);

    public IList<ClassDeclarationSyntax> SerializeContextClasses { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax cds
            || cds.AttributeLists.Count == 0)
        {
            return;
        }

        var att = cds.AttributeLists
                     .SelectMany(list => list.Attributes)
                     .FirstOrDefault(att =>
                     {
                         var text = att.GetText().ToString();
                         var index = text.IndexOf('(');
                         if (index >= 0)
                         {
                             text = text.Substring(0, index);
                         }
                         return text.Equals(shortAttrName) || text.Equals(fullAttrName);
                     });

        if (att != default)
        {
            SerializeContextClasses.Add(cds);
        }
    }
}
