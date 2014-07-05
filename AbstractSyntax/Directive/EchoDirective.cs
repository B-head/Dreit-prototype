using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class EchoDirective : Element
    {
        public Element Exp { get; private set; }

        public EchoDirective(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
        }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
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

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if(Exp != null && Exp.IsVoidReturn)
            {
                CompileError("invalid-void");
            }
        }
    }
}
