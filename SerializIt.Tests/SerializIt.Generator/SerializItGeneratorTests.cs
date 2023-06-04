using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializIt.Generator;

namespace SerializIt.Tests.SerializIt.Generator;

[TestClass]
public class SerializItGeneratorTests
{
    [TestMethod]
    public void TestJsonSerializerGenerator()
    {
        var source = SerializerTestData.SerializerContextCode
            .Replace("[Serializer]", "[Serializer(ESerializers.Json)]")
            .Replace("[SerializerOptions]", "[JsonOptions()]");
        GenerateAndTest(source);
    }

    [TestMethod]
    public void TestYamlSerializerGenerator()
    {
        var source = SerializerTestData.SerializerContextCode
            .Replace("[Serializer]", "[Serializer(ESerializers.Yaml)]")
            .Replace("[SerializerOptions]", "[YamlOptions()]");
        GenerateAndTest(source);
    }

    [TestMethod]
    public void TestXmlSerializerGenerator()
    {
        var source = SerializerTestData.SerializerContextCode
            .Replace("[Serializer]", "[Serializer(ESerializers.Xml)]")
            .Replace("[SerializerOptions]", "[XmlOptions()]");
        GenerateAndTest(source);
    }

    private static void GenerateAndTest(string source)
    {
        var compilation = SerializerTestData.CreateCompilation(source);
        var diag = compilation.GetDiagnostics();
        Assert.IsTrue(diag.IsEmpty, diag.FirstOrDefault()?.GetMessage());

        SerializItGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { generator });
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        Assert.IsTrue(diagnostics.IsEmpty, diagnostics.FirstOrDefault()?.GetMessage());
        Assert.AreEqual(5, outputCompilation.SyntaxTrees.Count());

        AssetSerializerClass(outputCompilation.SyntaxTrees.Skip(2).First().ToString(), "DataType");
        AssetSerializerClass(outputCompilation.SyntaxTrees.Skip(3).First().ToString(), "ContainerType");
        AssetSerializerClass(outputCompilation.SyntaxTrees.Skip(4).First().ToString(), "RecordType");
    }

    private static void AssetSerializerClass(string code, string name)
    {
        Assert.IsTrue(code.Contains($"public class {name}Serializer"));
        Assert.IsFalse(code.Contains("IsHidden"));
    }
}
