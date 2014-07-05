using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Postfix : MonadicExpression
    {
        public Postfix(TextPosition tp, TokenType op, Element child)
            :base(tp, op, child)
        {

        }

        public override Scope ReturnType
        {
            get
            {
                var c = Child.Reference.FindDataType() as ClassSymbol;
                if (c == null)
                {
                    return Root.Unknown;
                }
                return c.TypeofSymbol;
            }
        }
    }
}
