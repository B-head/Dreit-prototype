using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class MonadicExpression : Element
    {
        public TokenType Operator { get; private set; }
        public Element Exp { get; private set; }

        protected MonadicExpression(TextPosition tp, TokenType op, Element exp)
            :base(tp)
        {
            Operator = op;
            Exp = exp;
            AppendChild(Exp);
        }

        protected override string ElementInfo
        {
            get { return Enum.GetName(typeof(TokenType), Operator); }
        }

        internal override void CheckSemantic()
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    CompileError("require-expression");
                    continue;
                }
            }
            base.CheckSemantic();
        }
    }
}
