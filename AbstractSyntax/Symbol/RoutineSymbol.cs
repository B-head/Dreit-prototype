using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public class RoutineSymbol : Scope
    {
        public List<DataType> ArgumentType { get; set; }
        public DataType ReturnType { get; set; }
    }
}
