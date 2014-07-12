using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Condition : DyadicExpression
    {
        private Scope _CallScope;

        public Condition(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Root.OpManager.Find(Operator, VirtualRight.ReturnType, Left.ReturnType);
                }
                return _CallScope;
            }
        }

        public override Scope ReturnType
        {
            get { return CallScope.CallReturnType; }
        }

        public bool IsLeftConnection
        {
            get { return Parent is Condition; }
        }

        public bool IsRightConnection
        {
            get { return Right is Condition; }
        }

        public Element VirtualRight
        {
            get
            {
                if (IsRightConnection)
                {
                    var c = (Condition)Right;
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
