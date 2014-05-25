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
        public GenericSymbol GenericType { get; set; }

        public CastPragma()
        {
            GenericType = new GenericSymbol { Name = "T" };
        }

        public override int Count
        {
            get { return 1; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return GenericType;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        //todo ジェネリクスの構文で型検査をする。
        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new DataType[] { GenericType, GenericType }, new GenericSymbol[] { GenericType });
        }
    }
}
