using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax.Expression
{
    public class LeftAssign : DyadicExpression
    {
        public override DataType DataType
        {
            get { return Right.DataType; }
        }

        internal override void CheckSyntax()
        {
            if(Right != null && Right is RightAssign)
            {
                CompileError("式中では割り当て演算子の向きが揃っている必要があります。");
            }
            if (Left != null && Left is RightAssign)
            {
                CompileError("式中では割り当て演算子の向きが揃っている必要があります。");
            }
            else if(Left != null && !Left.IsAssignable)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
            base.CheckSyntax();
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (Right != null && Left != null)
            {
                DeclateVariant temp = Left as DeclateVariant;
                if(temp != null)
                {
                    temp.SetDataType(DataType);
                }
            }
        }
    }
}
