using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightAssign : DyadicExpression
    {
        public override DataType DataType
        {
            get { return Left.DataType; }
        }

        internal override void CheckSyntax()
        {
            if (Right != null && !Right.IsAssignable)
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
                DeclateVariant temp = Right as DeclateVariant;
                if (temp != null)
                {
                    temp.SetDataType(DataType);
                }
            }
        }
    }
}
