using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightAssign : DyadicExpression
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
            DeclateVariant temp = Right as DeclateVariant;
            if (temp != null)
            {
                temp.SetDataType(Left.DataType);
            }
            if (Left.DataType != Right.DataType && ConversionRoutine is UndefinedSymbol)
            {
                CompileError("not-convertable-right");
            }
        }
    }
}
