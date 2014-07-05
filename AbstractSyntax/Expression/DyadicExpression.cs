using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class DyadicExpression : Element
    {
        public TokenType Operator { get; private set; }
        public Element Left { get; private set; }
        public Element Right { get; private set; }

        protected DyadicExpression(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Left;
                    case 1: return Right;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        protected override string ElementInfo
        {
            get { return Operator.ToString(); }
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
