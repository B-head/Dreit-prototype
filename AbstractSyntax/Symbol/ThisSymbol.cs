using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ThisSymbol : Scope
    {
        private DeclateClass _DataType;

        public ThisSymbol(DeclateClass dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override DataType DataType
        {
            get { return _DataType; }
        }
    }
}
