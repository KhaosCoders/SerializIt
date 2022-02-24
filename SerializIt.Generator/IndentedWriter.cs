using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerializIt;

public class IndentedWriter
{
    public bool AutoIndentCode { get; set; }

    public string IndentChars
    {
        get => _indentChars;
        set
        {
            if (IndentChars == value)
            {
                return;
            }
            _indentChars = value;
            _indents.Clear();

            if (Indent <= 0)
            {
                return;
            }

            _sbIndent ??= new();
            _sbIndent.Length = 0;
            for (var i = 1; i <= Indent; i++)
            {
                _sbIndent.Append(_indentChars);
                _indents.Add(_sbIndent.ToString());
            }
            _sbIndent.Length = 0;
        }
    }

    public int Indent
    {
        get => _indent;
        set
        {
            _indent = value > 0 ? value : 0;
            if (_indent <= _indents.Count)
            {
                return;
            }

            _sbIndent ??= new();
            _sbIndent.Length=0;
            if (_indents.Count > 0)
            {
                _sbIndent.Append(_indents[_indents.Count - 1]);
            }

            for (var i = _indents.Count; i < _indent; i++)
            {
                _sbIndent.Append(_indentChars);
                _indents.Add(_sbIndent.ToString());
            }
            _sbIndent.Length = 0;
        }
    }

    [ThreadStatic] private static StringBuilder _sbIndent;

    private readonly StringBuilder _sb = new();
    private readonly Stack<bool> _layerFlags = new();
    private bool _newLine;
    private readonly List<string> _indents = new();
    private string _indentChars = "\t";
    private int _indent;

    public IndentedWriter()
    {
        AutoIndentCode = true;
    }

    /// <inheritdoc cref="StringBuilder.ToString()" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        _sb.ToString();

    public void Clear()
    {
        _sb.Length = 0;
        _newLine = false;
        _indent = 0;
        _layerFlags.Clear();
        IsLayerSet = false;
    }

    #region Indent

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void NoIndent()
    {
        _newLine = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndent()
    {
        if (_indent > 0)
        {
            _sb.Append(_indents[_indent - 1]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteIndentation()
    {
        if (_newLine && AutoIndentCode && _indent > 0)
        {
            this.AddIndent();
        }
        _newLine = false;
    }

    #endregion

    #region Write

    public void Write(string code)
    {
        WriteIndentation();
        _sb.Append(code);
    }

    public void Write(char chr)
    {
        WriteIndentation();
        _sb.Append(chr);
    }

    public void Write(char[] chr)
    {
        WriteIndentation();
        _sb.Append(chr);
    }

    public void Write(byte num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(sbyte num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(short num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(int num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(long num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(float num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(double num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(decimal num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(object obj)
    {
        WriteIndentation();
        _sb.Append(obj);
    }

    public void Write(bool value)
    {
        WriteIndentation();
        _sb.Append(value);
    }

    public void Write(uint num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    public void Write(ulong num)
    {
        WriteIndentation();
        _sb.Append(num);
    }

    #endregion

    #region NewLine

    public void NewLine()
    {
        WriteIndentation();
        _sb.AppendLine();
        _newLine = true;
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
        _layerFlags.Push(IsLayerSet);
        IsLayerSet = false;
    }

    public void EndLayer()
    {
        IsLayerSet = _layerFlags.Pop();
    }

    #endregion
}
