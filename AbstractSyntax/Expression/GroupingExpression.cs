using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class GroupingExpression : Element
    {
        public Element Exp { get; private set; }

        public GroupingExpression(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
            AppendChild(Exp);
        }

        public override Scope ReturnType
        {
            get { return Exp.ReturnType; }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    cmm.CompileError("require-expression", this);
                    continue;
                }
            }
        }
    }
}
