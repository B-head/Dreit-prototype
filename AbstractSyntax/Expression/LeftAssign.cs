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
        public TokenType Operator { get; set; }

        public override Element Left
        {
            get
            {
                return Access;
            }
            set
            {
                Access = value;
            }
        }

        public override Element Right
        {
            get
            {
                return (Element)Arguments[0];
            }
            set
            {
                if (value is TupleList)
                {
                    Arguments = (TupleList)value;
                }
                else
                {
                    Arguments = new TupleList(value);
                }
            }
        }

        public override TokenType CalculateOperator
        {
            get { return Operator ^ TokenType.LeftAssign; }
        }

        protected override string ElementInfo
        {
            get { return Operator.ToString(); }
        }

        internal override void CheckSemantic()
        {
            if(Right != null && Right is RightAssign)
            {
                CompileError("not-collide-assign");
            }
            if (Left != null && Left is RightAssign)
            {
                CompileError("not-collide-assign");
            }
            base.CheckSemantic();
        }
    }
}
