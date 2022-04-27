using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SerializIt.Generator.Helpers;
using SerializIt.Generator.Model;
using SerializIt.Generator.Serializers;

namespace SerializIt.Generator;

internal static class Collector
{
    /// <summary>
    /// Collects all information needed for the creation of a serializer
    /// </summary>
    internal static SerializationContext? CollectSerializationInfo(Compilation compilation, TypeDeclarationSyntax serializeContextClass)
    {
        var model = compilation.GetSemanticModel(serializeContextClass.SyntaxTree);
        var classSymbol = model.GetDeclaredSymbol(serializeContextClass);

        if (classSymbol == null)
        {
            return default;
        }

        // Collect serialization info
        SerializationContext serializationContext = new(classSymbol.Name,
            SymbolHelper.GetNamespaceName(classSymbol.ContainingNamespace) ?? string.Empty)
        {
            Accessibility = SymbolHelper.GetAccessor(classSymbol.DeclaredAccessibility)
        };

        // Serializer
        LoadSerializer(serializationContext, classSymbol);
        if (serializationContext.Serializer is null)
        {
            return default;
        }

        // Look for types to serialize
        foreach (var attribute in classSymbol.GetAttributes().Where(a =>
                     Names.SerializeTypeAttribute.Equals($"{SymbolHelper.GetNamespaceName(a.AttributeClass?.ContainingNamespace)}.{a.AttributeClass?.Name}")))
        {
            if (attribute.ConstructorArguments.Length < 1
                || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol targetTypeSymbol
                || targetTypeSymbol.Kind == SymbolKind.ErrorType)
            {
                continue;
            }

            serializationContext.SerializeTypes.Add(CollectTypeInfo(targetTypeSymbol));
        }

        return serializationContext;
    }

    private static SerializeType CollectTypeInfo(INamedTypeSymbol targetTypeSymbol)
    {
        SerializeType serializationInfo = new(targetTypeSymbol.ToTypeSymbol())
        {
            Accessibility = SymbolHelper.GetAccessor(targetTypeSymbol.DeclaredAccessibility)
        };

        // All public members can be serialized
        foreach (var memberSymbol in targetTypeSymbol.GetAllMembers(Accessibility.Public, SymbolKind.Property))
        {
            // public properties and fields
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

            // calculate next free order index for this member
            var index = serializationInfo.GetNextOrderIndex();

            // check member attributes
            foreach (var attribute in memberSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.Name is null)
                {
                    continue;
                }

                switch (attribute.AttributeClass.Name)
                {
                    case nameof(OrderAttribute) when attribute.ConstructorArguments.Length > 0:
                        // Overrides order of members
                        if (attribute.ConstructorArguments[0].Value is int order && order >= 0)
                        {
                            index = serializationInfo.GetNextOrderIndex(order);
                        }
                        break;
                    case nameof(SkipAttribute) when attribute.ConstructorArguments.Length > 0:
                        // Skip member if its value is default
                        if (attribute.ConstructorArguments[0].Value is bool ifDefault)
                        {
                            memberInfo.SkipIfDefault = ifDefault;
                        }
                        break;
                    case nameof(SkipAttribute):
                        // Always skip member
                        memberInfo.Skip = true;
                        break;
                }
            }
            memberInfo.Order = index;
            serializationInfo.Members.Add(memberInfo);
        }

        return serializationInfo;
    }

    internal static void LoadSerializer(SerializationContext serializationContext, ISymbol classSymbol)
    {
        try
        {
            // Look for a SerializerAttribute
            var attrSymbol = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(SerializerAttribute));
            if (attrSymbol == null)
            {
                return;
            }

            // Create an instance of the SerializerAttribute
            var attr = CodeActivator.Attribute<SerializerAttribute>(attrSymbol.ToString());
            if (attr is not null)
            {
                // Try getting the serializer type
                var serializerType = SerializerFactory.GetSerializerType(attr);
                if (serializerType != null && typeof(ISerializer).IsAssignableFrom(serializerType))
                {
                    // Create an instance of the serializer type
                    serializationContext.Serializer = (ISerializer)Activator.CreateInstance(serializerType);
                }

                // Assign a Namespace, if defined by user
                if (!string.IsNullOrWhiteSpace(attr.Namespace))
                {
                    serializationContext.SerializerNamespace = attr.Namespace;
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

        LoadSerializerOptions(serializationContext, classSymbol);
    }

    private static void LoadSerializerOptions(SerializationContext serializationContext, ISymbol classSymbol)
    {
        // Serializer should be loaded
        if (serializationContext.Serializer is null)
        {
            return;
        }

        // Serializer class needs a SerializerOptionsAttribute to tell us about its options
        var serializerType = serializationContext.Serializer.GetType();
        if (serializerType.GetCustomAttributes(typeof(SerializerOptionsAttribute), false).FirstOrDefault() is not SerializerOptionsAttribute serializerOptAttr)
        {
            return;
        }

        // Try find a fitting options attribute on the seralization context
        var optionsClassName = serializerOptAttr.OptionsType?.Name;
        var attrSymbol = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == optionsClassName);
        if (attrSymbol == null)
        {
            return;
        }

        // Create an instance of the options
        var attr = CodeActivator.Attribute<OptionsAttribute>(attrSymbol.ToString());
        if (attr is null)
        {
            return;
        }

        serializationContext.Serializer.Options = attr.Options;
    }
}
