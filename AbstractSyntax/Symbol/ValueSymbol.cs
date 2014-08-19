using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ValueSymbol : Element
    {
        public object Value { get; private set; }

        public ValueSymbol(object value)
        {
            Value = value;
        }
    }
}
