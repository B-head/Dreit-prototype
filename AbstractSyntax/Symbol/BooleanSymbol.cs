using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class BooleanSymbol : Scope
    {
        public bool Value { get; private set; }

        public BooleanSymbol(bool value)
        {
            Value = value;
        }
    }
}
