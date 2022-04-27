using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializIt.Generator.Helpers;

namespace SerializIt.Tests.SerializIt.Generator;

[TestClass]
public class CodeActivatorTests
{
    [TestMethod]
    public void TestSerializerAttribute()
    {
        var code = $"{typeof(SerializerAttribute).FullName}({typeof(ESerializers).FullName}.{nameof(ESerializers.Yaml)})";

        var att = CodeActivator.Attribute<SerializerAttribute>(code);

        Assert.IsNotNull(att);
        Assert.AreEqual(ESerializers.Yaml, att.Serializer);
    }

    [TestMethod]
    public void TestOptionsAttribute()
    {
        var code = $"{typeof(YamlOptionsAttribute).FullName}(true, true)";

        var att = CodeActivator.Attribute<YamlOptionsAttribute>(code);

        Assert.IsNotNull(att);
        Assert.IsTrue(att.YamlOptions.HasValue);

        var opt = att.YamlOptions.Value;
        Assert.IsTrue(opt.AddPreamble);
        Assert.IsTrue(opt.AddPostamble);
    }

    [TestMethod]
    public void TestFailure()
    {
        const string code = "will fail";

        var att = CodeActivator.Attribute<YamlOptionsAttribute>(code);

        Assert.IsNull(att);
    }
}
