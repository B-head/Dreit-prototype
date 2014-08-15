using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadSimplex : OverLoad
    {
        public Scope Symbol { get; private set; }

        public OverLoadSimplex(Scope symbol)
        {
            Symbol = symbol;
        }

        public override bool IsUndefined
        {
            get { return Symbol is ErrorRoutineSymbol; }
        }

        internal override Root Root
        {
            get { return Symbol.Root; }
        }

        internal override IEnumerable<Scope> TraversalChilds()
        {
            yield return Symbol;
        }

        internal override IEnumerable<VariantSymbol> TraversalVariant()
        {
            var variant = Symbol as VariantSymbol;
            if (variant != null)
            {
                yield return variant;
            }
        }

        internal override IEnumerable<AttributeSymbol> TraversalAttribute()
        {
            var attr = Symbol as AttributeSymbol;
            if (attr != null)
            {
                yield return attr;
            }
        }

        internal override IEnumerable<TypeSymbol> TraversalDataType()
        {
            var type = Symbol as TypeSymbol;
            if (type != null)
            {
                yield return type;
            }
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            foreach (var m in Symbol.GetTypeMatch(pars, args))
            {
                yield return m;
            }
        }
    }
}
