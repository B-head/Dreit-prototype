using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    public class ExpressionGrouping : MonadicExpression
    {
        public override DataType DataType
        {
            get { return Child.DataType; }
        }
    }
}
