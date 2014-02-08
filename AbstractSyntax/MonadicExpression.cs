using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public abstract class MonadicExpression : Element
    {
        public Element Child { get; set; }
        public TokenType Operation { get; set; }

        public override int ChildCount
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

        protected override string ElementInfo()
        {
            return base.ElementInfo() + (Operation == TokenType.Special ? string.Empty : Enum.GetName(typeof(TokenType), Operation));
        }

        internal override void CheckSemantic()
        {
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    CompileError("式が必要です。");
                    continue;
                }
            }
            base.CheckSemantic();
        }
    }
}
