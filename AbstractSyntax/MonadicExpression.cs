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
        public Element _Child { get; set; }
        public TokenType Operation { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return _Child;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string AdditionalInfo()
        {
            return Enum.GetName(typeof(TokenType), Operation);
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
