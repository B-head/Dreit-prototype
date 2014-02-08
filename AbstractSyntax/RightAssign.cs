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
        internal override void CheckSemantic()
        {
            if (Right != null && !Right.IsReference)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
            base.CheckSemantic();
        }

        internal override void CheckDataType()
        {
            if (Right != null && Left != null)
            {
                Scope temp = Left.GetDataType();
                Right.CheckDataTypeAssign(temp);
            }
            base.CheckDataType();
        }

        internal override Scope GetDataType()
        {
            return Left.GetDataType();
        }

        internal override void Translate()
        {
            Left.Translate();
            Right.TranslateAssign();
            Right.Translate();
        }
    }
}
