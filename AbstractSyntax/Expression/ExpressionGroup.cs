using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ExpressionGroup : MonadicExpression
    {
        public override IDataType ReturnType
        {
            get { return Child.ReturnType; }
        }
    }
}
