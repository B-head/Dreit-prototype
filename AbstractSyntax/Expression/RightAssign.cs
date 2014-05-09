using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightAssign : DyadicExpression, ICaller
    {
        private Scope _ConversionRoutine;

        public override DataType DataType
        {
            get { return Left.DataType; }
        }

        public Scope ConversionRoutine
        {
            get
            {
                if (_ConversionRoutine == null)
                {
                    _ConversionRoutine = Root.Conversion.Find(Left.DataType, Right.DataType);
                }
                return _ConversionRoutine;
            }
        }

        internal override void CheckSyntax()
        {
            if (!(Right is IAccess))
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
                CompileError("not-convertable-right");
            }
        }

        public DataType GetCallType()
        {
            DeclateVariant temp = Right as DeclateVariant;
            if (Left == null || temp == null)
            {
                return Root.Unknown;
            }
            return Left.DataType;
        }
    }
}
