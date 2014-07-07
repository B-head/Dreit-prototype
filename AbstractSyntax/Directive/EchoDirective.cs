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
            AppendChild(Exp);
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
