using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Calculate : DyadicExpression
    {
        private RoutineSymbol _CallRoutine;

        public Calculate(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Root.OpManager.FindDyadic(Operator, Left.ReturnType, Right.ReturnType);
                }
                return _CallRoutine;
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return Left.IsConstant && Right.IsConstant && CallRoutine.IsFunction; }
        }

        public override dynamic GenerateConstantValue()
        {
            var l = Left.GenerateConstantValue();
            var r = Right.GenerateConstantValue();
            switch(Operator)
            {
                case TokenType.Add: return l + r;
                case TokenType.Subtract: return l - r;
                case TokenType.Combine: return l + r;
                case TokenType.Multiply: return l * r;
                case TokenType.Divide: return l / r;
                case TokenType.Modulo: return l % r;
                default: throw new InvalidOperationException();
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallRoutine is ErrorRoutineSymbol && !TypeSymbol.HasAnyErrorType(Left.ReturnType, Right.ReturnType))
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
