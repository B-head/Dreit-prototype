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
        public CalculatePragmaType CalculateType { get; private set; }
        public GenericSymbol GenericType { get; set; }

        public CalculatePragma(string name, CalculatePragmaType type)
        {
            Name = "@@" + name;
            CalculateType = type;
            GenericType = new GenericSymbol(new TextPosition(), "T");
            _Attribute = new List<Scope>();
            AppendChild(GenericType);
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { GenericType, GenericType });
        }

        internal bool IsCondition
        {
            get
            {
                switch(CalculateType)
                {
                    case CalculatePragmaType.EQ:
                    case CalculatePragmaType.NE:
                    case CalculatePragmaType.LT:
                    case CalculatePragmaType.LE:
                    case CalculatePragmaType.GT:
                    case CalculatePragmaType.GE:
                        return true;
                }
                return false;
            }
        }

        internal Scope BooleanSymbol
        {
            get { return NameResolution("Boolean").FindDataType(); }
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        EQ,
        NE,
        LT,
        LE,
        GT,
        GE,
    }
}
