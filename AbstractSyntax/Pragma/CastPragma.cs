using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CastPragma : RoutineSymbol
    {
        public CastPragma()
        {
            Name = "@@cast";
            var r = new GenericSymbol(new TextPosition(), "R");
            var t = new GenericSymbol(new TextPosition(), "T");
            _Generics = new GenericSymbol[] { r, t };
            _ArgumentTypes = new Scope[] { t };
            _CallReturnType = r;
        }
    }
}
