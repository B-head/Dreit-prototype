using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class ReturnStatement : Element
    {
        public Element Exp { get; private set; }

        public ReturnStatement(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
            AppendChild(Exp);
        }
    }
}
