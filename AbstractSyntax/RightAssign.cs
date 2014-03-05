using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class RightAssign : DyadicExpression
    {
        internal override Scope DataType
        {
            get { return Left.DataType; }
        }

        internal override void CheckSyntax()
        {
            if (Right != null && !Right.IsAssignable)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
            base.CheckSyntax();
        }

        internal override void CheckDataType(Scope scope)
        {
            base.CheckDataType(scope);
            if (Right != null && Left != null)
            {
                Right.CheckDataTypeAssign(DataType);
            }
        }

        internal override void Translate(Translator trans)
        {
            Left.Translate(trans);
            Right.TranslateAssign(trans);
            Right.Translate(trans);
        }
    }
}
