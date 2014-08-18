using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class AssociatePair : Element
    {
        public Element Left { get; private set; }
        public Element Right { get; private set; }

        public AssociatePair(TextPosition tp, Element left, Element right)
            :base(tp)
        {
            Left = left;
            Right = right;
            AppendChild(Left);
            AppendChild(Right);
        }
    }
}
