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
        {
            Name = "void";
        }
    }
}
