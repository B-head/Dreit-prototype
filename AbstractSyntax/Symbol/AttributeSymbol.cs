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
        public AttributeType Attr { get; private set; }

        public AttributeSymbol(AttributeType attr)
        {
            Attr = attr;
        }

        public AttributeSymbol(string name, AttributeType attr)
        {
            Name = name;
            Attr = attr;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { });
        }
    }

    public enum AttributeType
    {
        None,
        Var,
        Let,
        Routine,
        Function,
        Class,
        Trait,
        Refer,
        Tyoeof,
        Static,
        Public,
        Protected,
        Private,
    }
}
