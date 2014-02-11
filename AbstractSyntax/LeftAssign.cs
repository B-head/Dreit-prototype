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
        protected override void CheckSyntax()
        {
            if(Right != null && Right is RightAssign)
            {
                CompileError("式中では割り当て演算子の向きが揃っている必要があります。");
            }
            if(Left != null && !Left.IsReference)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
        }

        internal override void CheckDataType(Scope scope)
        {
            if (Right != null && Left != null)
            {
                DataType = Right.DataType;
                Left.CheckDataTypeAssign(DataType);
            }
            base.CheckDataType(scope);
        }

        internal override void Translate()
        {
            Right.Translate();
            Left.TranslateAssign();
            Left.Translate();
        }
    }
}
