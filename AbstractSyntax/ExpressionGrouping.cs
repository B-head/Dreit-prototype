using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class ExpressionGrouping : MonadicExpression
    {
        internal override Scope DataType
        {
            get { return _Child.DataType; }
        }
    }
}
