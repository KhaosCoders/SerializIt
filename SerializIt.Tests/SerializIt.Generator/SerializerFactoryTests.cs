using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializIt.Generator.Helpers;
using SerializIt.Generator.Serializers;

namespace SerializIt.Tests.SerializIt.Generator;

[TestClass]
public class SerializerFactoryTests
{
    [TestMethod]
    public void TestSerializerFactory()
    {
        Assert.AreEqual(typeof(YamlSerializer), SerializerFactory.GetSerializerType(new SerializerAttribute(ESerializers.Yaml)));
        Assert.AreEqual(typeof(XmlSerializer), SerializerFactory.GetSerializerType(new SerializerAttribute(ESerializers.Xml)));
        Assert.AreEqual(typeof(JsonSerializer), SerializerFactory.GetSerializerType(new SerializerAttribute(ESerializers.Json)));
    }
}
