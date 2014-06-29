using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightAssign : Caller, IDyadicExpression
    {
        public Element Left { get; set; }
        public Element Right { get; set; }
        public TokenType Operator { get; set; }
        private Scope _ConversionRoutine;
        private TupleList _Arguments;

        public override Element Access
        {
            get { return Right; }
        }

        public override TupleList Arguments
        {
            get
            {
                if(_Arguments == null)
                {
                    if (Left is TupleList)
                    {
                        _Arguments = (TupleList)Left;
                    }
                    else
                    {
                        _Arguments = new TupleList(Left);
                    }
                }
                return _Arguments;
            }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Left;
                    case 1: return Right;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
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

        protected override string GetElementInfo()
        {
            return Operator.ToString();
        }

        internal override void CheckSyntax()
        {
            base.CheckSyntax();
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (Left.DataType != Right.DataType && ConversionRoutine is VoidSymbol)
            {
                CompileError("not-convertable-right");
            }
        }

        public override bool HasCallTarget(IElement element)
        {
            return Right == element;
        }

        public override IDataType GetCallType()
        {
            return Left.DataType;
        }
    }
}
