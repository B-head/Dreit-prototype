using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class OverLoadModify : OverLoad
    {
        public OverLoad Next { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }

        public OverLoadModify(OverLoad next, IReadOnlyList<TypeSymbol> parameters)
        {
            Next = next;
            Parameters = parameters;
        }

        public override bool IsUndefined
        {
            get { return Next.IsUndefined; }
        }

        internal override Root Root
        {
            get { return Next.Root; }
        }

        internal override IEnumerable<Scope> TraversalChilds()
        {
            return Next.TraversalChilds();
        }

        internal override IEnumerable<VariantSymbol> TraversalVariant()
        {
            return Next.TraversalVariant();
        }

        internal override IEnumerable<AttributeSymbol> TraversalAttribute()
        {
            return Next.TraversalAttribute();
        }

        internal override IEnumerable<OverLoadTypeMatch> TraversalDataType(IReadOnlyList<TypeSymbol> pars)
        {
            var newpars = pars.Concat(Parameters).ToList();
            return Next.TraversalDataType(newpars);
        }

        internal override IEnumerable<OverLoadCallMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            var newpars = pars.Concat(Parameters).ToList();
            return Next.TraversalCall(newpars, args);
        }

        public override string ToString()
        {
            return string.Format("Next = {{{0}}}, Parameters = !({1})", Next, Parameters.ToNames());
        }
    }
}
