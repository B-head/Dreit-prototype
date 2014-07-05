using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class ReturnDirective : Element
    {
        public Element Exp { get; private set; }

        public ReturnDirective(TextPosition tp, Element exp)
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
