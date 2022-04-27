using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SerializIt.Tests.SerializIt;

[TestClass]
public class XmlSerializerOptionsTests
{
    [XmlOptions(prettyPrint: true, lowercaseTags: true, addDocTag: false, indentChars: "x")]
    private class CustomTestOptions { }

    [XmlOptions()]
    private class DefaultTestOptions { }


    [TestMethod]
    public void TestDefaultXmlOptions()
    {
        var attr = typeof(DefaultTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is XmlOptionsAttribute)
            as XmlOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.XmlOptions.HasValue);

        var opt = attr.XmlOptions.Value;
        Assert.IsFalse(opt.PrettyPrint);
        Assert.IsFalse(opt.LowercaseTags);
        Assert.IsTrue(opt.AddDocTag);
        Assert.IsNull(opt.IndentChars);
        Assert.IsFalse(opt.UseParallel);
    }


    [TestMethod]
    public void TestCustomXmlOptions()
    {
        var attr = typeof(CustomTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is XmlOptionsAttribute)
            as XmlOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.XmlOptions.HasValue);

        var opt = attr.XmlOptions.Value;
        Assert.IsTrue(opt.PrettyPrint);
        Assert.IsTrue(opt.LowercaseTags);
        Assert.IsFalse(opt.AddDocTag);
        Assert.AreEqual("x", opt.IndentChars);
        Assert.IsFalse(opt.UseParallel);
    }

}
