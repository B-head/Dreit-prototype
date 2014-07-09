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

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if(Exp != null && Exp.IsVoidReturn)
            {
                cmm.CompileError("invalid-void", this);
            }
        }
    }
}
