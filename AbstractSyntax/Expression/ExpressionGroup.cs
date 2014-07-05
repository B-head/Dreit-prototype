using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ExpressionGroup : MonadicExpression
    {
        public ExpressionGroup(TextPosition tp, Element child)
            :base(tp, TokenType.Unknoun, child)
        {

        }

        public override IDataType ReturnType
        {
            get { return Child.ReturnType; }
        }
    }
}
