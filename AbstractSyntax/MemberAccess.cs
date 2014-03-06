using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class MemberAccess : DyadicExpression
    {
        public MemberAccess()
        {
            Operation = TokenType.Access;
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

        public void TranslateAccess(Translator trans)
        {
            Left.Translate(trans);
            var temp = Right as MemberAccess;
            if(temp != null)
            {
                temp.TranslateAccess(trans);
            }
        }
    }
}
