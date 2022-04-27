using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializIt.CodeAnalysis;

namespace SerializIt.Tests.SerializIt;

[TestClass]
public class TypeSymbolTests
{
    [TestMethod]
    public void TestCompare()
    {
        var symBase = new TypeSymbol()
        {
            Name = "Test1",
            Namespace = "SerializIt"
        };
        var symEqual = new TypeSymbol()
        {
            Name = "Test1",
            Namespace = "SerializIt"
        };
        var symDifferentName = new TypeSymbol()
        {
            Name = "Test2",
            Namespace = "SerializIt"
        };

        var symDifferentNamespace = new TypeSymbol()
        {
            Name = "Test1",
            Namespace = "Other"
        };

        Assert.AreEqual(0, symBase.CompareTo(symEqual));
        Assert.AreNotEqual(0, symBase.CompareTo(symDifferentName));
        Assert.AreNotEqual(0, symBase.CompareTo(symDifferentNamespace));
        Assert.AreEqual(-1, symBase.CompareTo("Test"));
    }
}
