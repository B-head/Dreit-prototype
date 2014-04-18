using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax
{
    public class MemberAccess : DyadicExpression
    {
        public MemberAccess()
        {
            Operator = TokenType.Access;
        }

        public override Scope DataType
        {
            get { return Right.DataType; }
        }

        public override Scope Reference
        {
            get
            {
                var ident = Right as IdentifierAccess;
                var member = Right as MemberAccess;
                if (ident != null)
                {
                    return ident.Reference;
                }
                else if (member != null)
                {
                    return member.Reference;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            Left.SpreadReference(scope);
            Right.SpreadReference(Left.DataType);
        }
    }
}
