using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class TemplateInstance : Element
    {
        public Element Access { get; private set; }
        public TupleList Arguments { get; private set; }

        public TemplateInstance(TextPosition tp, Element acs, TupleList args)
            : base(tp)
        {
            Access = acs;
            Arguments = args;
        }

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Access;
                    case 1: return Arguments;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }
    }
}
