using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class ReturnDirective : Element
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
