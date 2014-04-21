using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax.Pragma
{
    public class CalculatePragma : DeclateRoutine
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
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
