using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax.Expression
{
    public abstract class MonadicExpression : Element
    {
        public Element Child { get; set; }
        public TokenType Operator { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Child;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string AdditionalInfo()
        {
            return Enum.GetName(typeof(TokenType), Operator);
        }

        internal override void CheckSyntax()
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    CompileError("式が必要です。");
                    continue;
                }
            }
            base.CheckSyntax();
        }
    }
}
