using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class ExpressionGrouping : MonadicExpression
    {
        public override FullName GetDataType()
        {
            return Child.GetDataType();
        }
    }
}
