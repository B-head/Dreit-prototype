using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class DefaultSymbol : RoutineSymbol
    {
        public DefaultSymbol()
            : base(RoutineType.Function, TokenType.Unknoun)
        {
        }

        public DefaultSymbol(string name, ClassSymbol parent)
            :base(RoutineType.Routine, TokenType.Unknoun)
        {
            Name = name;
            _CallReturnType = parent;
        }
    }
}
