using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    abstract class DyadicExpression : Element
    {
        public Element Left { get; set; }
        public Element Right { get; set; }
        public TokenType Operation { get; set; }

        public override int ChildCount
        {
            get { return 2; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + (Operation == TokenType.Special ? string.Empty : Enum.GetName(typeof(TokenType), Operation));
        }

        public override void CheckSemantic()
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
