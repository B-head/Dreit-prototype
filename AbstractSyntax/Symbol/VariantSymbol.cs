using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public class VariantSymbol : Scope
    {
        protected DataType _DataType;

        public override DataType DataType
        {
            get { return _DataType; }
        }
    }
}
