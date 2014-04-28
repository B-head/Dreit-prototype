using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CastPragma : DeclateRoutine, IPragma
    {
        public CastPragma()
        {
            Argument = new TupleList();
            Block = new DirectiveList();
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
