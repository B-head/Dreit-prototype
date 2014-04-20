using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax.Expression
{
    public class MemberAccess : DyadicExpression
    {
        public MemberAccess()
        {
            Operator = TokenType.Access;
        }

        public override DataType DataType
        {
            get { return Right.DataType; }
        }

        public override Scope Reference
        {
            get { return Right.Reference; }
        }

        internal override void SpreadReference(Scope scope)
        {
            Left.SpreadReference(scope);
            Right.SpreadReference(Left.DataType);
        }
    }
}
