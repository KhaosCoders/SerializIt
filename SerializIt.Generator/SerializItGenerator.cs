using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SerializIt.Generator.Helpers;
using SerializIt.Generator.Model;
using SerializIt.Generator.Serializers;

namespace SerializIt.Generator;

[Generator]
public partial class SerializItGenerator : IIncrementalGenerator
{
    static SerializItGenerator()
    {
        AppDomain.CurrentDomain.UnhandledException += static (s, e) =>
        {
#if LOGS
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled Exception");
#endif
        };

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
                        Log.Debug($"Assembly not found in {Environment.NewLine}{string.Join(Environment.NewLine, assemblies.AsEnumerable())}");
                    }
#endif
                    return asm;
                }
            }
            catch (Exception ex)
            {
#if LOGS
                Log.Fatal(ex, "Failed to resolve assembly");
#endif
            }
            return null;
        };
    }

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
                        static (s, _) => IsSyntaxTargetForGeneration(s),
                        static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                    .Where(static m => m is not null)
                    .Collect());

            context.RegisterImplementationSourceOutput(provider, this.Emit);
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to Initialize Generator");
#endif
            throw;
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        try
        {
            //#if LOGS
            //        Log.Debug("Visit syntax node {0}: {1}", node.Kind(), node);
            //#endif
            return node is TypeDeclarationSyntax { AttributeLists: { Count: > 0 } };
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to check syntax node");
#endif
            throw;
        }
    }

    static TypeDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
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

    private void Emit(SourceProductionContext context,
        (Compilation compilation, ImmutableArray<TypeDeclarationSyntax?> types) source)
    {
        try
        {
#if LOGS
            Log.Debug("Emit {0} contexts: {1}{2}", source.types.Length, Environment.NewLine, string.Join(Environment.NewLine, source.types));
#endif
            var compilation = source.compilation;

            foreach (var serializeContextClass in source.types)
            {
                if (serializeContextClass == null)
                {
                    continue;
                }

                var model = compilation.GetSemanticModel(serializeContextClass.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(serializeContextClass);

                if (classSymbol == null)
                {
                    continue;
                }

                // Collect serialization info
                SerializationContext serializationContext = new(classSymbol.Name,
                    SymbolHelper.GetNamespaceName(classSymbol.ContainingNamespace) ?? string.Empty);
                serializationContext.Accessibility = SymbolHelper.GetAccessor(classSymbol.DeclaredAccessibility);

                // Serializer
                InitSerializer(serializationContext, classSymbol);

                // Collect types
                if (serializationContext.SerializeTypes == null)
                {
                    return;
                }

                foreach (var attribute in classSymbol.GetAttributes().Where(a =>
                             Names.SerializeTypeAttribute.Equals($"{SymbolHelper.GetNamespaceName(a.AttributeClass?.ContainingNamespace)}.{a.AttributeClass?.Name}")))
                {
                    if (attribute.ConstructorArguments.Length < 1
                        || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol targetTypeSymbol
                        || targetTypeSymbol.Kind == SymbolKind.ErrorType)
                    {
                        continue;
                    }

                    serializationContext.SerializeTypes.Add(CollectSerializationInfo(targetTypeSymbol));
                }

                // Augment SerializationContext
                ContextAugmentator.Augment(serializationContext, context);

                // Build serializers
                foreach (var seralizeType in serializationContext.SerializeTypes)
                {
                    AddSerializerClass(serializationContext, seralizeType, context);
                }
            }
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to emit sources");
#endif
            throw;
        }
    }

    private void InitSerializer(SerializationContext serializationContext, INamedTypeSymbol classSymbol)
    {
        try
        {
            // Load Serializer
            var attrSymbol = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(SerializerAttribute));
            if (attrSymbol == null)
            {
                return;
            }

            var attr = CodeActivator.LoadAttribute(attrSymbol.ToString());
            if (attr is SerializerAttribute serializerAttr)
            {
                var serializerType = SerializerFactory.GetSerializerType(serializerAttr);
                if (serializerType != null && typeof(ISerializer).IsAssignableFrom(serializerType))
                {
                    serializationContext.Serializer = (ISerializer)Activator.CreateInstance(serializerType);
                }

                if (serializerAttr.Namespace != null)
                {
                    serializationContext.SerializerNamespace = serializerAttr.Namespace;
                }
            }
        }
        catch (Exception e)
        {
#if LOGS
            Log.Fatal(e, "Failed to init serializer");
#endif
            throw;
        }
        finally
        {
            // Default to JSON
            serializationContext.Serializer ??= new JsonSerializer() { JsonOptions = new JsonSerializerOptions() };
        }

        InitSerializerOptions(serializationContext, classSymbol);
    }

    private void InitSerializerOptions(SerializationContext serializationContext, INamedTypeSymbol classSymbol)
    {
        if (serializationContext.Serializer is null)
        {
            return;
        }

        var serializerType = serializationContext.Serializer.GetType();

        if (serializerType.GetCustomAttributes(typeof(SerializerOptionsAttribute), false).FirstOrDefault() is not SerializerOptionsAttribute seriOptAttr)
        {
            return;
        }

        var optionsClassName = seriOptAttr.OptionsType?.Name;
        var attrSymbol = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == optionsClassName);
        if (attrSymbol == null)
        {
            return;
        }

        var attr = CodeActivator.LoadAttribute(attrSymbol.ToString());
        if (attr is not OptionsAttribute optAttr)
        {
            return;
        }

        serializationContext.Serializer.Options = optAttr.Options;
    }

    private void AddSerializerClass(SerializationContext serializationContext, SerializeType serializationInfo, SourceProductionContext context)
    {
        var generator = new SerializationGenerator();
        var code = generator.GenerateSerializer(serializationContext, serializationInfo);
        context.AddSource($"{serializationContext.SerializerNamespace}.{serializationInfo.SerializerName}.generated.cs", code);
    }

    private SerializeType CollectSerializationInfo(INamedTypeSymbol targetTypeSymbol)
    {
        SerializeType serializationInfo = new(targetTypeSymbol.ToTypeSymbol());
        serializationInfo.Accessibility = SymbolHelper.GetAccessor(targetTypeSymbol.DeclaredAccessibility);

        foreach (var memberSymbol in targetTypeSymbol.GetAllMembers(Accessibility.Public, SymbolKind.Property))
        {
            var memberInfo = memberSymbol switch
            {
                IPropertySymbol propertySymbol => new SerializeMember(memberSymbol.Name, propertySymbol.Type.ToTypeSymbol()),
                IFieldSymbol fieldSymbol => new SerializeMember(memberSymbol.Name, fieldSymbol.Type.ToTypeSymbol()),
                _ => null
            };
            if (memberInfo == null)
            {
                continue;
            }

            var index = serializationInfo.GetNextOrderIndex();

            foreach (var attribute in memberSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.Name is null)
                {
                    continue;
                }

                switch (attribute.AttributeClass.Name)
                {
                    case nameof(OrderAttribute) when attribute.ConstructorArguments.Length > 0:
                        index = (int)attribute.ConstructorArguments[0].Value;
                        index = serializationInfo.GetNextOrderIndex(index);
                        break;
                    case nameof(SkipAttribute) when attribute.ConstructorArguments.Length > 0 && (bool)attribute.ConstructorArguments[0].Value:
                        memberInfo.SkipIfDefault = (bool)attribute.ConstructorArguments[0].Value;
                        break;
                    case nameof(SkipAttribute):
                        memberInfo.Skip = true;
                        break;
                }
            }
            memberInfo.Order = index;
            serializationInfo.Members.Add(memberInfo);
        }

        return serializationInfo;
    }
}
