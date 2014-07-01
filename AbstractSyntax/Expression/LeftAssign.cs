using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class LeftAssign : Caller, IDyadicExpression
    {
        public Element Left { get; set; }
        public Element Right { get; set; }
        public TokenType Operator { get; set; }
        private TupleList _Arguments;

        public override Element Access
        {
            get { return Left; }
        }

        public override TupleList Arguments
        {
            get
            {
                if (_Arguments == null)
                {
                    if (Right is TupleList)
                    {
                        _Arguments = (TupleList)Right;
                    }
                    else
                    {
                        _Arguments = new TupleList(Right);
                    }
                }
                return _Arguments;
            }
        }

        public override TokenType CalculateOperator
        {
            get { return Operator ^ TokenType.LeftAssign; }
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

        protected override string GetElementInfo()
        {
            return Operator.ToString();
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
            base.CheckSyntax();
        }

        public override bool HasCallTarget(IElement element)
        {
            return Left == element;
        }

        public override IDataType GetCallType()
        {
            return Right.ReturnType;
        }
    }
}
