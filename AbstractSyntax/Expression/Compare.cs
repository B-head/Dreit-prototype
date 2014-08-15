using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Compare : DyadicExpression
    {
        private RoutineSymbol _CallRoutine;

        public Compare(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Root.OpManager.FindDyadic(Operator, Left.ReturnType, VirtualRight.ReturnType);
                }
                return _CallRoutine;
            }
        }

        public override bool IsConstant
        {
            get { return Left.IsConstant && Right.IsConstant && CallRoutine.IsFunction; }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public bool IsLeftConnection
        {
            get { return Parent is Compare; }
        }

        public bool IsRightConnection
        {
            get { return Right is Compare; }
        }

        public Element VirtualRight
        {
            get
            {
                if (IsRightConnection)
                {
                    var c = (Compare)Right;
                    return c.Left;
                }
                else
                {
                    return Right;
                }
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallRoutine is ErrorRoutineSymbol && !SyntaxUtility.HasAnyErrorType(Left.ReturnType, Right.ReturnType))
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
