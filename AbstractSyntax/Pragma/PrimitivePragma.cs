using AbstractSyntax.Daclate;
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

    public enum PrimitivePragmaType
    {
        NotPrimitive,
        Object,
        String,
        Boolean,
        Integer8,
        Integer16,
        Integer32,
        Integer64,
        Natural8,
        Natural16,
        Natural32,
        Natural64,
        Binary32,
        Binary64,
    }
}
