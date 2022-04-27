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
        Assert.IsTrue(diag.IsEmpty);

        SerializItGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { generator });
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        Assert.IsTrue(diagnostics.IsEmpty);
        Assert.AreEqual(4, outputCompilation.SyntaxTrees.Count());
    }
}
