using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class ArrayLiteral : Element
    {
        public IReadOnlyList<Element> Values { get; private set; }

        public ArrayLiteral(TextPosition tp, IReadOnlyList<Element> values)
            :base(tp)
        {
            Values = values;
            AppendChild(Values);
        }
    }
}
