using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SerializIt.Generator.Helpers;

namespace SerializIt.Tests.SerializIt.Generator;

internal static class SerializerTestData
{
    public const string SerializerContextCode = @"
using System.Collections.Generic;
using SerializIt;

namespace Tests {

public record RecordType(int ID, string Name)
{
    [Skip]
    public bool IsHidden { get; set; }
};

public class DataType
{
    public int ID { get; set; }

    [Skip(true)]
    public string Name { get; set; }

    [Skip]
    public bool IsHidden { get; set; }
}

public class ContainerType
{
    public DataType[] ArrayField;

    public List<DataType> FieldProperty { get; set; }
}

[Serializer]
[SerializerOptions]
[SerializeType(typeof(DataType))]
[SerializeType(typeof(ContainerType))]
[SerializeType(typeof(RecordType))]
public partial class Serializer{ }

}

namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit { }
}
";

    public static Compilation CreateCompilation(string source)
    {
        List<MetadataReference> references = new();
        references.AddMetadataRef(typeof(ESerializers));
        references.AddMetadataRef(typeof(object));
        references.AddMetadataRef(typeof(Enumerable));
        references.AddMetadataRef(typeof(Stack<int>));
        references.AddMetadataRef(typeof(Accessibility));
        references.AddMetadataRef(typeof(CSharpCompilation));
        references.AddMetadataRef("netstandard");
        references.AddMetadataRef("System.Runtime");

        var assemblyName = Path.GetRandomFileName();
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        return CSharpCompilation.Create(assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
