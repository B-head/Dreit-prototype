using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ThisSymbol : Scope
    {
        private DeclateClass _DataType;

        public ThisSymbol(DeclateClass dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override DataType DataType
        {
            get { return _DataType; }
        }

        internal override TypeMatchResult TypeMatch(IReadOnlyList<DataType> type)
        {
            if (type.Count == 0)
            {
                return TypeMatchResult.PerfectMatch;
            }
            else if (type.Count == 1)
            {
                if (type[0] == DataType)
                {
                    return TypeMatchResult.PerfectMatch;
                }
                else
                {
                    return TypeMatchResult.MissMatchType;
                }
            }
            else
            {
                return TypeMatchResult.MissMatchCount;
            }
        }
    }
}
