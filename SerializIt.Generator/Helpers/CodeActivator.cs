using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SerializIt.Generator.Helpers;

public static class CodeActivator
{
    public static T? Attribute<T>(string code) where T : Attribute
    {
        if (string.IsNullOrWhiteSpace(code))
        {
#if LOGS
            Log.Warn("Attribute code is empty: {0}", code);
#endif
            return default;
        }

#if LOGS
        Log.Warn("Create instance of attribute: {0}", code);
#endif

        // Referenced assemblies
        List<MetadataReference> references = new();
        references.AddMetadataRef(typeof(ESerializers));
        references.AddMetadataRef(typeof(object));
        references.AddMetadataRef(typeof(Enumerable));
        references.AddMetadataRef(typeof(Stack<int>));
        references.AddMetadataRef(typeof(Accessibility));
        references.AddMetadataRef(typeof(CSharpCompilation));
        references.AddMetadataRef("netstandard");
        references.AddMetadataRef("System.Runtime");

        // ScriptOptions with IsCollectible=true
        var options = ScriptOptions.Default
            .WithReferences(references);
        var isCollectableMeth = typeof(ScriptOptions).GetMethod("WithIsCollectible", BindingFlags.Instance | BindingFlags.NonPublic);
        isCollectableMeth?.Invoke(options, new object[] { true });

        // Create Script
        var script = CSharpScript.Create($"return new {code};", options);

        // Check for errors in script
        var diagnostics = script.Compile();
        var failures = diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

        if (failures.Any())
        {
#if LOGS
            Log.Warn("Compile errors:");
            foreach (var diagnostic in failures)
            {
                Log.Warn("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }
#endif
            return default;
        }

        // Run script
        var task = script.RunAsync();
        task.Wait();

        // Return result
        var state = task.Result;
        return state.ReturnValue as T;
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
