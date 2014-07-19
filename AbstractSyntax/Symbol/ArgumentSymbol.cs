using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ArgumentSymbol : VariantSymbol
    {
        protected ArgumentSymbol(TextPosition tp)
            : base(tp, false)
        {

        }
    }
}
