using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class MonadicExpression : Element
    {
        public TokenType Operator { get; private set; }
        public Element Child { get; private set; }

        protected MonadicExpression(TextPosition tp, TokenType op, Element child)
            :base(tp)
        {
            Operator = op;
            Child = child;
        }

        public override int Count
        {
            get { return 1; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Child;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
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
