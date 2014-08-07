using AbstractSyntax.Declaration;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ThisSymbol : VariantSymbol
    {
        public ThisSymbol(ClassSymbol dataType)
            :base(false)
        {
            Name = "this";
            _Attribute = new List<Scope>();
            _DataType = dataType;
        }
    }
}
