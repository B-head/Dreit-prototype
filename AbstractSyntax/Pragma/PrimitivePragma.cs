using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class PrimitivePragma : ClassSymbol
    {
        public PrimitivePragmaType BasePrimitiveType { get; private set; }

        public PrimitivePragma(string name, PrimitivePragmaType type)
        {
            Name = "@@" + name;
            BasePrimitiveType = type;
        }
    }
}
