using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CastPragma : DeclateRoutine
    {
        public CastPragma()
        {
            Argument = new TupleList();
            Block = new DirectiveList();
        }

        public override bool IsPragma
        {
            get { return true; }
        }

        internal override TypeMatchResult TypeMatch(List<DataType> type)
        {
            if (type.Count != 2)
            {
                return TypeMatchResult.MissMatchCount;
            }
            else if (type[0] == type[1])
            {
                return TypeMatchResult.MissMatchType;
            }
            return TypeMatchResult.PerfectMatch;
        }
    }
}
