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
        public DefaultSymbol(string name, ClassSymbol parent)
            :base(RoutineType.Routine, TokenType.Unknoun)
        {
            Name = name;
            _Attribute = new List<Scope>();
            _ArgumentTypes = new List<Scope>();
            _CallReturnType = parent;
        }
    }
}
