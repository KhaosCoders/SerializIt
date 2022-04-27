using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SerializIt.Tests.SerializIt;

[TestClass]
public class JsonSerializerOptionsTests
{
    [JsonOptions(prettyPrint: true, useSingeQuotes: true, indentChars: "x", skipNullValues: false)]
    private class CustomTestOptions { }

    [JsonOptions()]
    private class DefaultTestOptions { }


    [TestMethod]
    public void TestDefaultJsonOptions()
    {
        var attr = typeof(DefaultTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is JsonOptionsAttribute)
            as JsonOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.JsonOptions.HasValue);

        var opt = attr.JsonOptions.Value;
        Assert.IsFalse(opt.PrettyPrint);
        Assert.IsFalse(opt.UseSingleQuotes);
        Assert.AreEqual('"', opt.Quotes);
        Assert.IsNull(opt.IndentChars);
        Assert.IsTrue(opt.SkipNullValues);
        Assert.IsFalse(opt.UseParallel);
    }


    [TestMethod]
    public void TestCustomJsonOptions()
    {
        var attr = typeof(CustomTestOptions)
            .GetCustomAttributes(false)
            .FirstOrDefault(a => a is JsonOptionsAttribute)
            as JsonOptionsAttribute;
        Assert.IsNotNull(attr);
        Assert.IsTrue(attr.JsonOptions.HasValue);

        var opt = attr.JsonOptions.Value;
        Assert.IsTrue(opt.PrettyPrint);
        Assert.IsTrue(opt.UseSingleQuotes);
        Assert.AreEqual('\'', opt.Quotes);
        Assert.AreEqual("x", opt.IndentChars);
        Assert.IsFalse(opt.SkipNullValues);
        Assert.IsFalse(opt.UseParallel);
    }

}
