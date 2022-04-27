using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SerializIt.Tests.SerializIt;

[TestClass]
public class IndentedWriterTests
{
    private readonly string NL;

    public IndentedWriterTests()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        NL = System.Environment.NewLine;
    }

    #region Indent

    [TestMethod]
    public void TestDefaultIndentChars()
    {
        IndentedWriter writer = new();
        writer.Indent++;
        writer.AddIndent();

        var result = writer.ToString();
        Assert.AreEqual("\t", result);
    }

    [TestMethod]
    public void TestCustomIndentChars()
    {
        IndentedWriter writer = new()
        {
            IndentChars = "  "
        };
        writer.Indent++;
        writer.AddIndent();

        var result = writer.ToString();
        Assert.AreEqual("  ", result);
    }

    [TestMethod]
    public void TestIndent()
    {
        IndentedWriter writer = new();
        writer.Indent += 3;
        writer.AddIndent();

        var result = writer.ToString();
        Assert.AreEqual("\t\t\t", result);
    }

    [TestMethod]
    public void TestIndentNeeded()
    {
        IndentedWriter writer = new()
        {
            IndentChars = "."
        };
        writer.NewLine();
        writer.Indent++;
        writer.Write("x");

        var result = writer.ToString();
        Assert.AreEqual($"{NL}.x", result);
    }

    [TestMethod]
    public void TestIndentNoNeeded()
    {
        IndentedWriter writer = new()
        {
            IndentChars = "."
        };
        writer.Indent++;
        writer.Write("x");

        var result = writer.ToString();
        Assert.AreEqual("x", result);
    }

    [TestMethod]
    public void TestNoIndent()
    {
        IndentedWriter writer = new()
        {
            IndentChars = "."
        };
        writer.NewLine();
        writer.NoIndent();
        writer.Indent++;
        writer.Write("x");

        var result = writer.ToString();
        Assert.AreEqual($"{NL}x", result);
    }

    #endregion

    #region NewLine

    [TestMethod]
    public void TestNewLine()
    {
        IndentedWriter writer = new();
        writer.NewLine();

        var result = writer.ToString();
        Assert.AreEqual($"{NL}", result);
    }

    [TestMethod]
    public void TestNewLineWithIndent()
    {
        IndentedWriter writer = new()
        {
            IndentChars = "."
        };
        writer.NewLine();
        writer.Indent++;
        writer.NewLine();

        var result = writer.ToString();
        Assert.AreEqual($"{NL}.{NL}", result);
    }

    [TestMethod]
    public void TestNewLineNeeded()
    {
        IndentedWriter writer = new();
        writer.Write("x");
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"x{NL}", result);
    }

    [TestMethod]
    public void TestNewLineNotNeeded()
    {
        IndentedWriter writer = new();
        writer.NewLineIfNeeded();
        writer.Write("x");

        var result = writer.ToString();
        Assert.AreEqual("x", result);
    }

    #endregion

    #region Write

    [TestMethod]
    public void TestWriteString()
    {
        IndentedWriter writer = new();
        writer.Write("x");
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"x{NL}", result);
    }

    [TestMethod]
    public void TestWriteChar()
    {
        IndentedWriter writer = new();
        writer.Write('x');
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"x{NL}", result);
    }

    [TestMethod]
    public void TestWriteCharArray()
    {
        IndentedWriter writer = new();
        writer.Write(new char[] { 'x', 'y', 'z' });
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"xyz{NL}", result);
    }

    [TestMethod]
    public void TestWriteByte()
    {
        IndentedWriter writer = new();
        writer.Write((byte)1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteSByte()
    {
        IndentedWriter writer = new();
        writer.Write((sbyte)1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteShort()
    {
        IndentedWriter writer = new();
        writer.Write((short)1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteInteger()
    {
        IndentedWriter writer = new();
        writer.Write(1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteUInteger()
    {
        IndentedWriter writer = new();
        writer.Write((uint)1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteLong()
    {
        IndentedWriter writer = new();
        writer.Write(1L);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteULong()
    {
        IndentedWriter writer = new();
        writer.Write((ulong)1);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1{NL}", result);
    }

    [TestMethod]
    public void TestWriteFloat()
    {
        IndentedWriter writer = new();
        writer.Write(1.2f);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1.2{NL}", result);
    }

    [TestMethod]
    public void TestWriteDouble()
    {
        IndentedWriter writer = new();
        writer.Write(1.2d);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1.2{NL}", result);
    }

    [TestMethod]
    public void TestWriteDecimal()
    {
        IndentedWriter writer = new();
        writer.Write((decimal)1.2);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"1.2{NL}", result);
    }

    [TestMethod]
    public void TestWriteObject()
    {
        StringBuilder sb = new();
        sb.Append('x');

        IndentedWriter writer = new();
        writer.Write(sb);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"x{NL}", result);
    }

    [TestMethod]
    public void TestWriteBool()
    {
        IndentedWriter writer = new();
        writer.Write(true);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"True{NL}", result);
    }

    [TestMethod]
    public void TestWriteBlock()
    {
        StringBuilder sb = new();
        sb.AppendLine("1");
        sb.AppendLine("2");
        sb.AppendLine("3");
        var block = sb.ToString();

        IndentedWriter writer = new();
        writer.Indent++;
        writer.NewLine();
        writer.WriteBlock(block);
        writer.NewLineIfNeeded();

        var result = writer.ToString();
        Assert.AreEqual($"{NL}\t1{NL}\t2{NL}\t3{NL}", result);
    }

    #endregion

    #region Layers

    [TestMethod]
    public void TestLayers()
    {
        IndentedWriter writer = new();
        Assert.IsFalse(writer.IsLayerSet);

        writer.IsLayerSet = true;
        writer.StartLayer();
        Assert.IsFalse(writer.IsLayerSet);

        writer.StartLayer();
        Assert.IsFalse(writer.IsLayerSet);

        writer.EndLayer();
        Assert.IsFalse(writer.IsLayerSet);

        writer.EndLayer();
        Assert.IsTrue(writer.IsLayerSet);
    }

    #endregion
}
