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

        public override DataType DataType
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
