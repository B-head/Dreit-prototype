using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class VoidSymbol : DataType
    {
        public VoidSymbol()
        {
            Name = "void";
        }
    }
}
