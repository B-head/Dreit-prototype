using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class AttributeSymbol : Scope, IDataType
    {
        public AttributeType Attr { get; private set; }

        public AttributeSymbol(AttributeType attr)
        {
            Attr = attr;
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
        }
    }

    public enum AttributeType
    {
        None,
        Static,
        Public,
        Protected,
        Private,
    }
}
