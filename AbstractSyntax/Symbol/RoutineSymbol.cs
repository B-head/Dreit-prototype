using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineSymbol : Scope
    {
        protected List<DataType> _ArgumentType;
        protected DataType _ReturnType;

        internal override TypeMatchResult TypeMatch(IReadOnlyList<DataType> type)
        {
            if (ArgumentType.Count != type.Count)
            {
                return TypeMatchResult.MissMatchCount;
            }
            else
            {
                for (int i = 0; i < ArgumentType.Count; i++)
                {
                    if (ArgumentType[i] != type[i])
                    {
                        return TypeMatchResult.MissMatchType;
                    }
                }
            }
            return TypeMatchResult.PerfectMatch;
        }

        public virtual List<DataType> ArgumentType
        {
            get { return _ArgumentType; }
        }

        public virtual DataType ReturnType
        {
            get { return _ReturnType; }
        }
    }
}
