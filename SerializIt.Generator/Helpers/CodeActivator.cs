using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SerializIt.Generator.Helpers;

public static class CodeActivator
{
    public static T? Attribute<T>(string code) where T : Attribute
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return default;
        }

        // Ensure attribute syntax
        if (code[0] != '[')
        {
            code = $"[{code.Replace("Attribute(", "(")}]";
        }

        // compile dummy class with attribute
        var objWithAttr = Class<object>($"namespace Generated {{ {code} public class Dummy {{ }} }}", "Generated.Dummy");

        // get dummy class type and extract attribute
        var objType = objWithAttr?.GetType();
        return objType?.GetCustomAttributes(false)[0] as T;
    }

    public static T? Class<T>(string code, string fullName) where T : class
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return default;
        }

        List<MetadataReference> references = new();
        references.AddMetadataRef(typeof(ESerializers));
        references.AddMetadataRef(typeof(object));
        references.AddMetadataRef(typeof(Enumerable));
        references.AddMetadataRef(typeof(Stack<int>));
        references.AddMetadataRef(typeof(Accessibility));
        references.AddMetadataRef(typeof(CSharpCompilation));
        references.AddMetadataRef("netstandard");
        references.AddMetadataRef("System.Runtime");

        // compile class as dll
        var assemblyName = Path.GetRandomFileName();
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(assemblyName,
                                         new[] { syntaxTree },
                                                   references,
                                                   new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // emit bits to MemoryStream
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // compilation failed
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
            // load assembly from memory
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());

            // create an instance for the compiled type
            var type = assembly.GetType(fullName);
            return Activator.CreateInstance(type) as T;
        }

        return default;
    }

    internal static void AddMetadataRef(this List<MetadataReference> list, Type type) =>
        list.Add(MetadataReference.CreateFromFile(type.Assembly.Location));

    internal static void AddMetadataRef(this List<MetadataReference> list, string asmName)
    {
        var asm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), a => a.GetName().Name.Equals(asmName));
        if (asm != null)
        {
            list.Add(MetadataReference.CreateFromFile(asm.Location));
        }
    }
}
