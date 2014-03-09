using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class ExpressionGrouping : MonadicExpression
    {
        public override Scope DataType
        {
            get { return Child.DataType; }
        }
    }
}
