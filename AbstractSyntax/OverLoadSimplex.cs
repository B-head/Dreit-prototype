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
        public Element Symbol { get; private set; }

        public OverLoadSimplex(Element symbol)
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
            var scope = Symbol as Scope;
            if (scope != null)
            {
                yield return scope;
            }
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

        internal override IEnumerable<TypeSymbol> TraversalDataType(IReadOnlyList<TypeSymbol> pars)
        {
            var type = Symbol as TypeSymbol;
            if (type != null)
            {
                yield return type;
            }
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            var inst = new List<GenericsInstance>();
            foreach (var m in Symbol.GetTypeMatch(inst, pars, args))
            {
                yield return m;
            }
        }

        public override string ToString()
        {
            return string.Format("Symbol = {{{0}}}", Symbol);
        }
    }
}
