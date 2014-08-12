using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class RangeLiteral : Element
    {
        public Element Left { get; private set; }
        public Element Right { get; private set; }
        public bool IsLeftOpen { get; private set; }
        public bool IsRightOpen { get; private set; }

        public RangeLiteral(TextPosition tp, Element left, Element right, bool isLeftOpen = false, bool isRightOpen = false)
            :base(tp)
        {
            Left = left;
            Right = right;
            IsLeftOpen = isLeftOpen;
            IsRightOpen = isRightOpen;
            AppendChild(Left);
            AppendChild(Right);
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (Left == null)
            {
                cmm.CompileError("require-expression", this);
            }
            if (Right == null)
            {
                cmm.CompileError("require-expression", this);
            }
        }
    }
}
