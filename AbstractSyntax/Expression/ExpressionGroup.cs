using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ExpressionGroup : MonadicExpression
    {
        public override DataType DataType
        {
            get { return Child.DataType; }
        }
    }
}
