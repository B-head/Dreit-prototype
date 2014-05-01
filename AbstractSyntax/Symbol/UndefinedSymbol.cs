using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class UndefinedSymbol : DataType
    {
        public UndefinedSymbol()
        {
            Name = "@@undefined";
        }
    }
}
