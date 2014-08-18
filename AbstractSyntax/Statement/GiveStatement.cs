using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class GiveStatement : Element
    {
        public Element Exp { get; private set; }

        public GiveStatement(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
            AppendChild(Exp);
        }
    }
}
