using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ExpressionGroup : MonadicExpression
    {
        public ExpressionGroup(TextPosition tp, Element exp)
            :base(tp, TokenType.Unknoun, exp)
        {

        }

        public override Scope ReturnType
        {
            get { return Exp.ReturnType; }
        }
    }
}
