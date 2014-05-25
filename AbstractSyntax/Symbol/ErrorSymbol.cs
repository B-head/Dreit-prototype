using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ErrorSymbol : Scope, IDataType
    {
        public ErrorSymbol()
        {
            Name = "error";
        }
    }
}
