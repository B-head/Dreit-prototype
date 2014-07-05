using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class GenericSymbol : Scope
    {   
        public GenericSymbol(TextPosition tp, string name)
            :base(tp)
        {
            Name = name;
        }

        public override bool IsDataType
        {
            get { return true; }
        }
    }
}
