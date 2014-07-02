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

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Root.OpManager[Operator].Find(VirtualRight.ReturnType, Left.ReturnType);
                }
                return _CallScope;
            }
        }

        public override IDataType ReturnType
        {
            get { return CallScope.ReturnType; }
        }

        public bool IsConnection
        {
            get { return Right is Condition; }
        }

        public IElement VirtualRight
        {
            get
            {
                if (IsConnection)
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

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if (CallScope is ErrorSymbol)
            {
                CompileError("impossible-calculate");
            }
        }
    }
}
