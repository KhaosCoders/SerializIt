using System;
using System.Collections.Generic;
using System.Text;

namespace SerializIt;

public class ExtStringBuilder
{
    public bool AutoIndentCode { get; set; }
    public string Indentation { get; set; }
    public string IndentChars { get; private set; }
    public int Indent
    {
        get => IndentChars.Length / Indentation.Length;
        set => IndentChars = value > 0
            ? new StringBuilder(Indentation.Length * value)
                .Insert(0, Indentation, value)
                .ToString()
            : string.Empty;
    }

    readonly StringBuilder _stringBuilder;
    readonly Stack<bool> _layerFlags;
    bool _currentLayerFlag;
    bool _newLine;

    public ExtStringBuilder()
    {
        _stringBuilder = new();
        _layerFlags = new();
        AutoIndentCode = true;
        Indentation = "\t";
        Indent = 0;
    }

    /// <inheritdoc cref="StringBuilder.ToString" />
    public override string ToString() =>
        _stringBuilder.ToString();

    #region Indent

    public ExtStringBuilder NoIndent()
    {
        _newLine = false;
        return this;
    }

    public ExtStringBuilder AddIndent()
    {
        _stringBuilder.Append(IndentChars);
        return this;
    }

    public ExtStringBuilder IncreaseIndent()
    {
        Indent++;
        return this;
    }

    public ExtStringBuilder DecreaseIndent()
    {
        Indent--;
        return this;
    }

    public ExtStringBuilder SetIndentation(string chars)
    {
        Indentation = chars;
        return this;
    }

    private void HandleAutoIndent()
    {
        if (_newLine && AutoIndentCode && Indent > 0)
        {
            this.AddIndent();
        }
        _newLine = false;
    }

    #endregion

    #region Append

    public ExtStringBuilder Append(Action<ExtStringBuilder> fAppender)
    {
        fAppender(this);
        return this;
    }

    public ExtStringBuilder Append(string code)
    {
        HandleAutoIndent();
        _stringBuilder.Append(code);
        return this;
    }

    public ExtStringBuilder Append(char chr)
    {
        HandleAutoIndent();
        _stringBuilder.Append(chr);
        return this;
    }

    public ExtStringBuilder Append(byte num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(sbyte num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(short num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(int num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(long num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(float num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(double num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(decimal num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(object obj)
    {
        HandleAutoIndent();
        _stringBuilder.Append(obj);
        return this;
    }

    public ExtStringBuilder Append(bool value)
    {
        HandleAutoIndent();
        _stringBuilder.Append(value);
        return this;
    }

    public ExtStringBuilder Append(StringBuilder sb)
    {
        HandleAutoIndent();
        _stringBuilder.Append(sb);
        return this;
    }

    public ExtStringBuilder Append(ushort num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(uint num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    public ExtStringBuilder Append(ulong num)
    {
        HandleAutoIndent();
        _stringBuilder.Append(num);
        return this;
    }

    #endregion

    #region AppendLine

    public ExtStringBuilder AppendLine(string code)
    {
        HandleAutoIndent();
        _stringBuilder.AppendLine(code);
        _newLine = true;
        return this;
    }

    #endregion

    #region AppendBlock

    public ExtStringBuilder AppendBlock(string code)
    {
        int startIndex = 0;
        int nextIndex()
        {
            int index = code.IndexOf('\r', startIndex);
            if (index >= 0)
            {
                return index;
            }

            index = code.IndexOf('\n', startIndex);
            if (index >= 0)
            {
                return index;
            }

            return code.Length;
        }
        int index = nextIndex();
        while (startIndex < index)
        {
            string line = code.Substring(startIndex, index - startIndex);
            AppendLine(line);

            if (index == code.Length)
            {
                break;
            }

            startIndex = index + 1;
            if (code.Length > startIndex && code[index] == '\r' && code[index + 1] == '\n')
            {
                startIndex++;
            }
            index = nextIndex();
        }
        return this;
    }

    #endregion

    #region Conditional

    public ExtStringBuilder Conditional(Func<bool> condition, Action<ExtStringBuilder> met, Action<ExtStringBuilder> unmet = null)
    {
        if (condition())
        {
            met(this);
        }
        else
        {
            unmet?.Invoke(this);
        }
        return this;
    }

    #endregion

    #region Layers

    public ExtStringBuilder StartLayer()
    {
        _layerFlags.Push(_currentLayerFlag);
        _currentLayerFlag = false;
        return this;
    }

    public ExtStringBuilder EndLayer()
    {
        _currentLayerFlag = _layerFlags.Pop();
        return this;
    }

    public ExtStringBuilder SetLayer(bool value = true)
    {
        _currentLayerFlag = value;
        return this;
    }

    public ExtStringBuilder IfLayer(Action<ExtStringBuilder> action)
    {
        if (_currentLayerFlag)
        {
            action(this);
        }
        return this;
    }

    #endregion
}
