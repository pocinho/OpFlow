// Copyright (c) 2026 Paulo Pocinho.

using System;

namespace PP.Toolkit.Operation.Generators.Emission;

internal static class CodeWriterExtensions
{
    public static void WriteBlock(this CodeWriter w, string header, Action body)
    {
        w.WriteLine(header);
        w.WriteLine("{");
        w.Indent();
        body();
        w.Unindent();
        w.WriteLine("}");
    }
}