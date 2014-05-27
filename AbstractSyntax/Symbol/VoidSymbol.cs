using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class VoidSymbol : Scope, IDataType
    {
        public VoidSymbol()
        {
            Name = "void";
        }
    }
}
