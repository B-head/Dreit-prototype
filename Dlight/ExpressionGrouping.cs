using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.CilTranslate;

namespace Dlight
{
    class ExpressionGrouping : MonadicExpression
    {
        public override Translator GetDataType()
        {
            return Child.GetDataType();
        }
    }
}
