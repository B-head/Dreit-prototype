using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax
{
    public class EchoDirective : Element
    {
        public Element Exp { get; set; }

        public override bool IsVoidValue
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 1; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Exp;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
