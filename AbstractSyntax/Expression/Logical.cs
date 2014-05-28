using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Logical : DyadicExpression
    {
        private Scope _CallScope;

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Root.OpManager[Operator].Find(Right.DataType, Left.DataType);
                }
                return _CallScope;
            }
        }

        public override IDataType DataType
        {
            get { return CallScope.DataType; }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (CallScope is ErrorSymbol)
            {
                CompileError("impossible-calculate");
            }
        }
    }
}
