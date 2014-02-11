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
        protected override void CheckSyntax()
        {
            if (Right != null && !Right.IsReference)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
        }

        internal override void CheckDataType(Scope scope)
        {
            if (Right != null && Left != null)
            {
                DataType = Left.DataType;
                Right.CheckDataTypeAssign(DataType);
            }
            base.CheckDataType(scope);
        }

        internal override void Translate()
        {
            Left.Translate();
            Right.TranslateAssign();
            Right.Translate();
        }
    }
}
