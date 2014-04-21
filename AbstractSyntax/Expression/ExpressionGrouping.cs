using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public class ExpressionGrouping : MonadicExpression
    {
        public override DataType DataType
        {
            get { return Child.DataType; }
        }
    }
}
