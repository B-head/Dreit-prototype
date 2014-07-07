using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Prefix : MonadicExpression
    {
        public Prefix(TextPosition tp, TokenType op, Element exp)
            :base(tp, op, exp)
        {

        }
    }
}
