using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class DefaultSymbol : RoutineSymbol
    {
        public DefaultSymbol(ClassSymbol parent)
        {
            _Attribute = new List<Scope>();
            _ArgumentTypes = new List<Scope>();
            _CallReturnType = parent;
        }
    }
}
