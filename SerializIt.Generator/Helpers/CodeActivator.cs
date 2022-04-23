using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SerializIt.Generator.Helpers
{
    internal static class CodeActivator
    {
        static CodeActivator()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);
            if (asmName.Name.Equals("SerializIt"))
            {
                return typeof(ESerializers).Assembly;
            }
            return null;
        }

        public static Attribute? LoadAttribute(string attrCode)
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

            var objType = objWithAttr?.GetType();
            return objType?.GetCustomAttributes(false)[0] as Attribute;
        }

        public static object? LoadClass(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var assemblyName = Path.GetRandomFileName();

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "netstandard").Location),
                MetadataReference.CreateFromFile(typeof(ESerializers).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stack<int>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Accessibility).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location)
            };

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var compilation = CSharpCompilation.Create(assemblyName,
                                             new[] { syntaxTree },
                                                       references,
                                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
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
