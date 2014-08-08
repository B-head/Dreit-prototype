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
                return Root.TypeManager.IssueTypeQualify(Exp.ReturnType, Root.Typeof); //todo Refer版にも対応する。
            }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant; }
        }
    }
}
