using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class LeftAssign : DyadicExpression
    {
        internal override void CheckSemantic()
        {
            if(Right != null && Right is RightAssign)
            {
                CompileError("式中では割り当て演算子の向きが揃っている必要があります。");
            }
            if(Left != null && !Left.IsReference)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
            base.CheckSemantic();
        }

        internal override void CheckDataType()
        {
            if (Right != null && Left != null)
            {
                Scope temp = Right.GetDataType();
                Left.CheckDataTypeAssign(temp);
            }
            base.CheckDataType();
        }

        internal override Scope GetDataType()
        {
            return Right.GetDataType();
        }

        internal override void Translate()
        {
            Right.Translate();
            Left.TranslateAssign();
            Left.Translate();
        }
    }
}
