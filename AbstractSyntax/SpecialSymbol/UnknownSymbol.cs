using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class UnknownSymbol : Scope
    {
        public override Scope ReturnType
        {
            get { return Root.Unknown; }
        }
    }
}
