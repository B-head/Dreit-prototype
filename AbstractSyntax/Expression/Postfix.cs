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
        public Postfix(TextPosition tp, TokenType op, Element exp)
            :base(tp, op, exp)
        {

        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if(Operator == TokenType.Refer)
                {
                    return Root.ClassManager.Issue(Root.Refer, new TypeSymbol[] { Exp.ReturnType }, new TypeSymbol[0]);
                }
                else if(Operator == TokenType.Typeof)
                {
                    return Root.ClassManager.Issue(Root.Typeof, new TypeSymbol[] { Exp.ReturnType }, new TypeSymbol[0]);
                }
                else if(Operator == TokenType.Reject)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant; }
        }
    }
}
