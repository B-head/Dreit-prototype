using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class LeftAssign : DyadicExpression, ICaller
    {
        private Scope _ConversionRoutine;

        public override DataType DataType
        {
            get { return Right.DataType; }
        }

        public Scope ConversionRoutine
        {
            get
            {
                if (_ConversionRoutine == null)
                {
                    _ConversionRoutine = Root.Conversion.Find(Right.DataType, Left.DataType);
                }
                return _ConversionRoutine;
            }
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
            else if(!(Left is IAccess))
            {
                CompileError("not-assignable");
            }
            base.CheckSyntax();
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (Right == null || Left == null)
            {
                return;
            }
            if (Left.DataType != Right.DataType && ConversionRoutine is VoidSymbol)
            {
                CompileError("not-convertable-left");
            }
        }

        public DataType GetCallType()
        {
            DeclateVariant temp = Left as DeclateVariant;
            if (Right == null || temp == null)
            {
                return Root.Unknown;
            }
            return Right.DataType;
        }
    }
}
