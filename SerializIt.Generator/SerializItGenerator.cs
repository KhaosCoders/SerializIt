using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace SerializIt.Generator;

[Generator]
public class SerializItGenerator : IIncrementalGenerator
{
    static SerializItGenerator()
    {
#if LOGS
        // Log error in Debug Mode
        AppDomain.CurrentDomain.UnhandledException += static (s, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled Exception");
        };
#endif

        AppDomain.CurrentDomain.AssemblyResolve += static (object _, ResolveEventArgs args) =>
        {
            try
            {
                var asmName = new AssemblyName(args.Name);
                if (asmName.Name.Equals("SerializIt"))
                {
#if LOGS
                    Log.Debug("Resolving SerializIt assembly...");
#endif
                    // reuse loaded SerializIt assembly in Visual Studio
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var asm = Array.Find(assemblies, a => a.GetName().Name == "SerializIt");
#if LOGS
                    if (asm == null)
                    {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
                        Log.Debug($"Assembly not found! Loaded assemblies: {Environment.NewLine}{string.Join(Environment.NewLine, assemblies.AsEnumerable())}");
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
                    }
#endif
                    return asm;
                }
            }
            catch (Exception ex)
            {
                // Only log exceptions in Debug Mode
#if LOGS
                Log.Fatal(ex, "Failed to resolve assembly");
#endif
            }
            return null;
        };
    }

    /// <summary>
    /// Initialize the generator pipeline
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
#if LOGS
            Log.Debug("Initialize SourceGenerator");
#endif
            var provider = context.CompilationProvider.Combine(
                context.SyntaxProvider
                    .CreateSyntaxProvider(
                        static (s, _) => SyntaxSelector.IsSyntaxTargetForGeneration(s),
                        static (ctx, _) => SyntaxSelector.GetSemanticTargetForGeneration(ctx))
                    .Where(static m => m is not null)
                    .Collect());

            context.RegisterImplementationSourceOutput(provider, Emitter.Emit);
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to Initialize Generator");
#endif
            throw;
        }
    }
}
