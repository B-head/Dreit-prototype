using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class VoidSymbol : ClassSymbol
    {
        public VoidSymbol()
            :base(ClassType.Unknown)
        {
            Name = "void";
        }
    }
}
