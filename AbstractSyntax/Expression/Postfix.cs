using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Postfix : MonadicExpression
    {
        public override IDataType ReturnType
        {
            get
            {
                var c = Child.Reference.GetDataType() as ClassSymbol;
                if (c == null)
                {
                    return Root.Unknown;
                }
                return c.TypeofSymbol;
            }
        }
    }
}
