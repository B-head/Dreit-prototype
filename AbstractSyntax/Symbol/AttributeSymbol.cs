using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class AttributeSymbol : Scope
    {
        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
        }
    }
}
