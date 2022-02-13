using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using SerializIt.Generator.Helpers;
using SerializIt.Generator.Model;
using SerializIt.Generator.Serializers;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SerializIt.Generator;

[Generator]
public class SerializItGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(context => AddStaticSerializItTypes(context));
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        var compilation = context.Compilation;

        foreach (var serializeContextClass in receiver.SerializeContextClasses)
        {
            var model = compilation.GetSemanticModel(serializeContextClass.SyntaxTree);
            var classSymbol = model.GetDeclaredSymbol(serializeContextClass);

            // Collect serialization info
            SerializationContext serializationContext = new(classSymbol.Name, SymbolHelper.GetNamespaceName(classSymbol.ContainingNamespace));
            serializationContext.Accessability = SymbolHelper.GetAccessor(classSymbol.DeclaredAccessibility);

            // Serializer
            InitSerializer(serializationContext, classSymbol);

            // Collect types
            foreach (var attribute in classSymbol.GetAttributes().Where(a => a.AttributeClass.Name == nameof(Resources.SerializeTypeAttribute)))
            {
                if (attribute.ConstructorArguments.Length < 1
                    || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol targetTypeSymbol)
                {
                    return;
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

    private void InitSerializer(SerializationContext serializationContext, INamedTypeSymbol classSymbol)
    {
        try
        {
            // Load Serializer
            var serializerAttr = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == nameof(SerializerAttribute));
            if (serializerAttr == null)
            {
                return;
            }

            var attr = CodeActivator.LoadAttribute(serializerAttr.ToString());
            if (attr != null)
            {
                var serializerType = SerializerFactory.GetSerializerType(attr);
                if (serializerType != null && typeof(ISerializer).IsAssignableFrom(serializerType))
                {
                    serializationContext.Serializer = (ISerializer)Activator.CreateInstance(serializerType);
                }

                var nsProp = attr.GetType().GetProperty(nameof(SerializerAttribute.Namespace), BindingFlags.Instance | BindingFlags.Public);
                if (nsProp != null)
                {
                    var ns = (string)nsProp.GetValue(attr);
                    if (ns != null)
                    {
                        serializationContext.SerializerNamespace = ns;
                    }
                }
            }
        }
        finally
        {
            // Default to JSON
            serializationContext.Serializer ??= new JsonSerializer() { Options = new JsonSerializerOptions() };
        }

        InitSerializerOptions(serializationContext, classSymbol);
    }

    private void InitSerializerOptions(SerializationContext serializationContext, INamedTypeSymbol classSymbol)
    {
        var serializerType = serializationContext.Serializer.GetType();

        if (serializerType.GetCustomAttributes(typeof(SerializerOptionsAttribute), false).FirstOrDefault() is not SerializerOptionsAttribute seriOptAttr)
        {
            return;
        }

        var optionsClassName = seriOptAttr.OptionsType.Name;
        var optionsAttr = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == optionsClassName);
        if (optionsAttr == null)
        {
            return;
        }

        var attr = CodeActivator.LoadAttribute(optionsAttr.ToString());
        if (attr == null)
        {
            return;
        }

        var attrProp = attr.GetType().GetProperty(nameof(JsonOptionsAttribute.Options), BindingFlags.Instance | BindingFlags.Public);
        var seriProp = serializerType.GetProperty(nameof(JsonOptionsAttribute.Options), BindingFlags.Instance | BindingFlags.Public);
        if (attrProp == null || seriProp == null)
        {
            return;
        }

        var options = attrProp.GetValue(attr);
        if (options == null)
        {
            return;
        }

        var opt = Caster.CastTo(ref options, seriProp.PropertyType);
        seriProp.SetValue(serializationContext.Serializer, opt);
    }

    private void AddStaticSerializItTypes(GeneratorPostInitializationContext context)
    {
        bool initCodeActivator = !CodeActivator.IsInitialized;

        void AddTypeFromResource(string name, string source)
        {
            if (source != null)
            {
                context.AddSource($"{name}.generated.cs", SourceText.From(source, Encoding.UTF8));

                if (initCodeActivator)
                {
                    CodeActivator.AddStaticCode(source);
                }
            }
        }

        AddTypeFromResource(nameof(Resources.ESerializers), Resources.ESerializers.Value);
        AddTypeFromResource(nameof(Resources.SerializerAttribute), Resources.SerializerAttribute.Value);
        AddTypeFromResource(nameof(Resources.SerializeTypeAttribute), Resources.SerializeTypeAttribute.Value);
        AddTypeFromResource(nameof(Resources.SkipAttribute), Resources.SkipAttribute.Value);
        AddTypeFromResource(nameof(Resources.OrderAttribute), Resources.OrderAttribute.Value);
        AddTypeFromResource(nameof(Resources.ExtStringBuilder), Resources.ExtStringBuilder.Value);
        AddTypeFromResource(nameof(Resources.ISerializerOptions), Resources.ISerializerOptions.Value);
        AddTypeFromResource(nameof(Resources.JsonSerializerOptions), Resources.JsonSerializerOptions.Value);
        AddTypeFromResource(nameof(Resources.YamlSerializerOptions), Resources.YamlSerializerOptions.Value);
        AddTypeFromResource(nameof(Resources.XmlSerializerOptions), Resources.XmlSerializerOptions.Value);
    }

    private void AddSerializerClass(SerializationContext serializationContext, SerializeType serializationInfo, GeneratorExecutionContext context)
    {
        var generator = new SerializationGenerator();
        string code = generator.GenerateSerializer(serializationContext, serializationInfo);
        context.AddSource($"{serializationContext.SerializerNamespace}.{serializationInfo.SerializerName}.generated.cs", code);
    }

    private SerializeType CollectSerializationInfo(INamedTypeSymbol targetTypeSymbol)
    {
        SerializeType serializationInfo = new(targetTypeSymbol);
        serializationInfo.Accessability = SymbolHelper.GetAccessor(targetTypeSymbol.DeclaredAccessibility);

        foreach (var memberSymbol in targetTypeSymbol.GetAllMembers(Accessibility.Public, SymbolKind.Property))
        {
            var memberInfo = memberSymbol switch
            {
                IPropertySymbol propertySymbol => new SerializeMember(memberSymbol.Name, propertySymbol.Type),
                IFieldSymbol fieldSymbol => new SerializeMember(memberSymbol.Name, fieldSymbol.Type),
                _ => null
            };
            if (memberInfo == null)
            {
                continue;
            }

            var index = serializationInfo.GetNextOrderIndex();

            foreach (var attribute in memberSymbol.GetAttributes())
            {
                switch (attribute.AttributeClass.Name)
                {
                    case nameof(Resources.OrderAttribute) when attribute.ConstructorArguments.Length > 0:
                        index = (int)attribute.ConstructorArguments[0].Value;
                        index = serializationInfo.GetNextOrderIndex(index);
                        break;
                    case nameof(Resources.SkipAttribute) when attribute.ConstructorArguments.Length > 0 && (bool)attribute.ConstructorArguments[0].Value:
                        memberInfo.SkipIfDefault = (bool)attribute.ConstructorArguments[0].Value;
                        break;
                    case nameof(Resources.SkipAttribute):
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
