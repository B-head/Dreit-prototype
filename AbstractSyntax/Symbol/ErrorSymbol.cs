using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ErrorSymbol : DataType
    {
        public ErrorSymbol()
        {
            Name = "error";
        }
    }
}
