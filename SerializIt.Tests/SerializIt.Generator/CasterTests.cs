using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializIt.Generator.Helpers;

namespace SerializIt.Tests.SerializIt.Generator;


[TestClass]
public class CasterTests
{
    private struct S1
    {
        public int id;
        public string name;
    }

    public struct S2
    {
        public int id;
        public string name;
    }

    [TestMethod]
    public void TestStructCast()
    {
        object s = new S1() { id = 1, name = "test" };
        var s1 = (S1)s;

        var x = Caster.CastTo(ref s, typeof(S2));
        var s2 = (S2)x;

        Assert.AreEqual(s1.id, s2.id);
        Assert.AreEqual(s1.name, s2.name);
    }
}
