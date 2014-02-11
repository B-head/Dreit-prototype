using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class MemberAccess : DyadicExpression
    {
        public MemberAccess()
        {
            Operation = TokenType.Access;
        }

        internal override Scope AccessType
        {
            get { return Right.AccessType; }
        }

        internal override void CheckDataType(Scope scope)
        {
            Left.CheckDataType(scope);
            Right.CheckDataType(Left.AccessType);
        }
    }
}
