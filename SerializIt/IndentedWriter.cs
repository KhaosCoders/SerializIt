using System.Runtime.CompilerServices;
using System.Text;

namespace SerializIt;

public class IndentedWriter
{
    public bool AutoIndentCode { get; set; }

    public string IndentChars
    {
        get => indentChars;
        set
        {
            if (IndentChars == value)
            {
                return;
            }
            indentChars = value;
            indents.Clear();

            if (Indent <= 0)
            {
                return;
            }

            sbIndent ??= new();
            sbIndent.Length = 0;
            for (var i = 1; i <= Indent; i++)
            {
                sbIndent.Append(indentChars);
                indents.Add(sbIndent.ToString());
            }
            sbIndent.Length = 0;
        }
    }

    public int Indent
    {
        get => indent;
        set
        {
            indent = value > 0 ? value : 0;
            if (indent <= indents.Count)
            {
                return;
            }

            sbIndent ??= new();
            sbIndent.Length = 0;
            if (indents.Count > 0)
            {
                sbIndent.Append(indents[indents.Count - 1]);
            }

            for (var i = indents.Count; i < indent; i++)
            {
                sbIndent.Append(indentChars);
                indents.Add(sbIndent.ToString());
            }
            sbIndent.Length = 0;
        }
    }

    [ThreadStatic] private static StringBuilder? sbIndent;

    private readonly StringBuilder sb = new();
    private readonly Stack<bool> layerFlags = new();
    private bool isNewLine;
    private bool needsNewLine;
    private readonly List<string> indents = new();
    private string indentChars = "\t";
    private int indent;

    public IndentedWriter()
    {
        AutoIndentCode = true;
    }

    /// <inheritdoc cref="StringBuilder.ToString()" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        sb.ToString();

    public void Clear()
    {
        sb.Length = 0;
        isNewLine = false;
        needsNewLine = false;
        indent = 0;
        layerFlags.Clear();
        IsLayerSet = false;
    }

    #region Indent

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void NoIndent()
    {
        isNewLine = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndent()
    {
        if (indent > 0)
        {
            sb.Append(indents[indent - 1]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteIndentation()
    {
        if (isNewLine && AutoIndentCode && indent > 0)
        {
            this.AddIndent();
        }
        isNewLine = false;
    }

    #endregion

    #region Write

    public void Write(string code)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(code);
    }

    public void Write(char chr)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(chr);
    }

    public void Write(char[] chr)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(chr);
    }

    public void Write(byte num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(sbyte num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(short num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(int num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(long num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(float num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(double num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(decimal num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(object obj)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(obj);
    }

    public void Write(bool value)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(value);
    }

    public void Write(uint num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    public void Write(ulong num)
    {
        needsNewLine = true;
        WriteIndentation();
        sb.Append(num);
    }

    #endregion

    #region NewLine

    public void NewLine()
    {
        WriteIndentation();
        sb.AppendLine();
        isNewLine = true;
        needsNewLine = false;
    }

    public void NewLineIfNeeded()
    {
        if (needsNewLine)
        {
            NewLine();
        }
    }

    #endregion

    #region WriteBlock

    public void WriteBlock(string code)
    {
        var startIndex = 0;
        int NextIndex()
        {
            var index = code.IndexOf('\r', startIndex);
            if (index >= 0)
            {
                return index;
            }

            index = code.IndexOf('\n', startIndex);
            return index >= 0 ? index : code.Length;
        }
        var index = NextIndex();
        while (startIndex < index)
        {
            var line = code.Substring(startIndex, index - startIndex);
            Write(line);
            NewLine();

            if (index == code.Length)
            {
                break;
            }

            startIndex = index + 1;
            if (code.Length > startIndex && code[index] == '\r' && code[index + 1] == '\n')
            {
                startIndex++;
            }
            index = NextIndex();
        }
    }

    #endregion

    #region Layers

    public bool IsLayerSet { get; set; }

    public void StartLayer()
    {
        layerFlags.Push(IsLayerSet);
        IsLayerSet = false;
    }

    public void EndLayer()
    {
        IsLayerSet = layerFlags.Pop();
    }

    #endregion
}
