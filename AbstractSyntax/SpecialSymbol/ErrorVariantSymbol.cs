using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class ErrorVariantSymbol : VariantSymbol
    {
        public ErrorVariantSymbol()
            :base(VariantType.Unknown)
        {

        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {

        }
    }
}
