using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class DyadicExpression : Element
    {
        public Element Left { get; set; }
        public Element Right { get; set; }
        public TokenType Operator { get; set; }

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
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override string ElementInfo
        {
            get
            {
                return Enum.GetName(typeof(TokenType), Operator);
            }
        }

        internal override void CheckSyntax()
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    CompileError("require-expression");
                    continue;
                }
            }
            base.CheckSyntax();
        }
    }
}
