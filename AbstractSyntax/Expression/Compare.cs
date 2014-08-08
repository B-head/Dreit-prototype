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
        private Scope _CallScope;

        public Compare(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Root.OpManager.FindDyadic(Operator, Left.ReturnType, VirtualRight.ReturnType);
                }
                return _CallScope;
            }
        }

        public override bool IsConstant
        {
            get { return Left.IsConstant && Right.IsConstant && ((RoutineSymbol)CallScope).IsFunction; }
        }

        public override Scope ReturnType
        {
            get { return CallScope.CallReturnType; }
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
            if (CallScope is ErrorSymbol)
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
