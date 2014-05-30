using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CalculatePragma : RoutineSymbol
    {
        public CalculatePragmaType Type { get; private set; }
        public GenericSymbol GenericType { get; set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
            GenericType = new GenericSymbol { Name = "T" };
        }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
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

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { GenericType, GenericType }, new GenericSymbol[] { GenericType });
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Eq,
        Ne,
        Lt,
        Le,
        Gt,
        Ge,
    }
}
