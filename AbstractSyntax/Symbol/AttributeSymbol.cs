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

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            yield return TypeMatch.MakeTypeMatch(Root.ConvManager, this, pars, new GenericSymbol[] { }, args, new Scope[] { });
        }
    }

    public enum AttributeType
    {
        None,
        Refer,
        Tyoeof,
        Contravariant,
        Covariant,
        ConstructorConstraint,
        ValueConstraint,
        ReferenceConstraint,
        Optional,
        Abstract,
        Virtual,
        Final,
        Static,
        Public,
        Internal,
        Protected,
        Private,
    }
}
