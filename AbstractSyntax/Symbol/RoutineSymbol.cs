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
        public virtual List<DataType> ArgumentType { get; set; }
        public virtual DataType ReturnType { get; set; }
    }
}
