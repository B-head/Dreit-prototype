using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class SwapExpression : DyadicExpression
    {
        public SwapExpression(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }
    }
}
