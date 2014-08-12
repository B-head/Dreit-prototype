using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class ThisSymbol : VariantSymbol
    {
        public ThisSymbol(ClassSymbol dataType)
            : base(VariantType.Var)
        {
            Name = "this";
            _Attribute = new List<Scope>();
            _DataType = dataType;
        }
    }
}
