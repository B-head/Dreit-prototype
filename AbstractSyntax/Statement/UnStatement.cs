using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class UnStatement : Element
    {
        public Element Exp { get; private set; }

        public UnStatement(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
        }

        public override int Count
        {
            get { return 1; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Exp;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }
    }
}
