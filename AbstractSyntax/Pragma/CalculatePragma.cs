using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CalculatePragma : DeclateRoutine, IPragma
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
            Argument = new TupleList();
            Block = new DirectiveList();
        }

        internal override TypeMatchResult TypeMatch(List<DataType> type)
        {
            if (type.Count != 2)
            {
                return TypeMatchResult.MissMatchCount;
            }
            else if (type[0] != type[1])
            {
                return TypeMatchResult.MissMatchType;
            }
            return TypeMatchResult.PerfectMatch;
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
    }
}
