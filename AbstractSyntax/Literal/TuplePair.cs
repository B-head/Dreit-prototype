using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class TuplePair : Element
    {
        public Identifier Tag { get; private set; }
        public Element Exp { get; private set; }

        public TuplePair(TextPosition tp, Identifier tag, Element exp)
            :base(tp)
        {
            Tag = tag;
            Exp = exp;
            AppendChild(Tag);
            AppendChild(Exp);
        }
    }
}
