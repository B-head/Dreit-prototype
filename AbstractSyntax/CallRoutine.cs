using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class CallRoutine : Element
    {
        public Element Access { get; set; }
        public Element Argument { get; set; }

        public override int Count
        {
            get { return 2; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return Access;
                case 1: return Argument;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
