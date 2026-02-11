// Copyright (c) 2026 Paulo Pocinho.

using System.Linq;
using System.Text;

namespace OpFlow.Generators.Emission;

internal sealed class CodeWriter
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel;
    private const string IndentString = "    ";

    public void Indent() => _indentLevel++;
    public void Unindent()
    {
        if (_indentLevel > 0) _indentLevel--;
    }

    public void WriteLine(string line = "")
    {
        if (line.Length > 0)
        {
            _sb.Append(string.Concat(Enumerable.Repeat(IndentString, _indentLevel)));
            _sb.AppendLine(line);
        }
        else
        {
            _sb.AppendLine();
        }
    }

    public void Write(string text)
    {
        _sb.Append(text);
    }

    public override string ToString() => _sb.ToString();
}