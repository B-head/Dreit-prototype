using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class UnknownSymbol : DataType
    {
        public UnknownSymbol()
        {
            Name = "@@unknown";
        }

        public override DataType DataType
        {
            get { return Root.Unknown; }
        }
    }
}
