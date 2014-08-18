using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class DictionaryLiteral : Element
    {
        public IReadOnlyList<AssociatePair> Pairs { get; private set; }

        public DictionaryLiteral(TextPosition tp, IReadOnlyList<AssociatePair> pairs)
            :base(tp)
        {
            Pairs = pairs;
            AppendChild(Pairs);
        }
    }
}
