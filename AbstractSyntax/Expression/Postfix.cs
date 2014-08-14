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

        public override Scope ReturnType
        {
            get
            {
                if(Operator == TokenType.Refer)
                {
                    return Root.TemplateInstanceManager.Issue(Root.Refer, Exp.ReturnType);
                }
                else if(Operator == TokenType.Typeof)
                {
                    return Root.TemplateInstanceManager.Issue(Root.Typeof, Exp.ReturnType);
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
