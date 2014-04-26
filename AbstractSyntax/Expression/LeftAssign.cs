using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
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
                CompileError("not-collide-assign");
            }
            if (Left != null && Left is RightAssign)
            {
                CompileError("not-collide-assign");
            }
            else if(Left != null && !Left.IsAssignable)
            {
                CompileError("not-assignable");
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
