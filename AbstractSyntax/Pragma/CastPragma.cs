﻿using AbstractSyntax.Daclate;
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
        public GenericSymbol GenericType { get; set; }

        public CastPragma()
        {
            Name = "@@cast";
            GenericType = new GenericSymbol(new TextPosition(), "T");
            _Attribute = new List<Scope>();
            AppendChild(GenericType);
        }

        //todo ジェネリクスの構文で型検査をする。
        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { GenericType, GenericType });
        }
    }
}
