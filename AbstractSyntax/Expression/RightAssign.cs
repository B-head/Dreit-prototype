﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax.Expression
{
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
                CompileError("割り当て可能な式である必要があります。");
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
