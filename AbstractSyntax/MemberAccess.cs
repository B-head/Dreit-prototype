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
            Operator = TokenType.Access;
        }

        public Scope Refer
        {
            get
            {
                var ident = Right as IdentifierAccess;
                var member = Right as MemberAccess;
                if (ident != null)
                {
                    return ident.Refer;
                }
                else if (member != null)
                {
                    return member.Refer;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        internal override Scope DataType
        {
            get { return Right.DataType; }
        }

        internal override void SpreadReference(Scope scope)
        {
            Left.SpreadReference(scope);
            Right.SpreadReference(Left.DataType);
        }
    }
}
