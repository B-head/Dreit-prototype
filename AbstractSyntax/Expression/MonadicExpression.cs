using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class MonadicExpression : Element, IMonadicExpression
    {
        public Element Child { get; set; }
        public TokenType Operator { get; set; }

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
                    case 0: return Child;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override string GetElementInfo()
        {
            return Enum.GetName(typeof(TokenType), Operator);
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
