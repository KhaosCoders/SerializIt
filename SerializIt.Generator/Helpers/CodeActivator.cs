using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SerializIt.Generator.Helpers
{
    internal static class CodeActivator
    {
        static List<SyntaxTree> _staticCodes = new();

        public static bool IsInitialized => _staticCodes.Count > 0;

        public static void AddStaticCode(params string[] staticCodes) =>
            Array.ForEach(staticCodes, code => _staticCodes.Add(CSharpSyntaxTree.ParseText(code)));

        public static Attribute LoadAttribute(string attrCode)
        {
            if (string.IsNullOrWhiteSpace(attrCode))
            {
                return null;
            }

            if (attrCode[0] != '[')
            {
                attrCode = $"[{attrCode.Replace("Attribute(", "(")}]";
            }

            var objWithAttr = LoadClass($"namespace Generated {{ {attrCode} public class Dummy {{ }} }}");
            if (objWithAttr == null)
            {
                return null;
            }

            var objType = objWithAttr.GetType();
            return objType.GetCustomAttributes(false)[0] as Attribute;
        }

        public static object LoadClass(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stack<int>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Accessibility).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location)
            };

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var compilation = CSharpCompilation.Create(assemblyName,
                                                       _staticCodes.Concat(new[] { syntaxTree }),
                                                       references,
                                                       options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var type = assembly.GetType("Generated.Dummy");
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
