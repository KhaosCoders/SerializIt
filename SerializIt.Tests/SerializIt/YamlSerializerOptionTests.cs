using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SerializIt.Tests.SerializIt;

[TestClass]
public class YamlSerializerOptionsTests
{
    [YamlOptions(addPreamble: true, addPostamble: true, indentChars: "x")]
    private class CustomTestOptions { }

    [YamlOptions()]
    private class DefaultTestOptions { }


    [TestMethod]
    public void TestDefaultYamlOptions()
    {
        var attr = typeof(DefaultTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is YamlOptionsAttribute)
            as YamlOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.YamlOptions.HasValue);

        var opt = attr.YamlOptions.Value;
        Assert.IsFalse(opt.AddPreamble);
        Assert.IsFalse(opt.AddPostamble);
        Assert.AreEqual("  ", opt.IndentChars);
        Assert.IsFalse(opt.UseParallel);
    }


    [TestMethod]
    public void TestCustomYamlOptions()
    {
        var attr = typeof(CustomTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is YamlOptionsAttribute)
            as YamlOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.YamlOptions.HasValue);

        var opt = attr.YamlOptions.Value;
        Assert.IsTrue(opt.AddPreamble);
        Assert.IsTrue(opt.AddPostamble);
        Assert.AreEqual("x", opt.IndentChars);
        Assert.IsFalse(opt.UseParallel);
    }

}
