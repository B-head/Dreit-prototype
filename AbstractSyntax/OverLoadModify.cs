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
        public IReadOnlyList<GenericsInstance> ScopeInstance { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }
        public bool ByMember { get; set; }
        public bool ByStatic { get; set; }

        private OverLoadModify(OverLoad next)
        {
            Next = next;
            Parameters = new List<TypeSymbol>();
            ScopeInstance = new List<GenericsInstance>();
        }

        public static OverLoadModify MakeScopeInstance(OverLoad ol, IReadOnlyList<GenericsInstance> scopeInstance)
        {
            var olm = ol as OverLoadModify;
            if(olm == null)
            {
                olm = new OverLoadModify(ol);
            }
            var s = olm.ScopeInstance.Concat(scopeInstance);
            olm.ScopeInstance = s.ToList();
            return olm;
        }

        public static OverLoadModify MakeParameters(OverLoad ol, IReadOnlyList<TypeSymbol> parameters)
        {
            var olm = ol as OverLoadModify;
            if (olm == null)
            {
                olm = new OverLoadModify(ol);
            }
            var s = olm.Parameters.Concat(parameters);
            olm.Parameters = s.ToList();
            return olm;
        }

        public static OverLoadModify MakeMember(OverLoad ol, bool byMember)
        {
            var olm = ol as OverLoadModify;
            if (olm == null)
            {
                olm = new OverLoadModify(ol);
            }
            olm.ByMember = byMember;
            return olm;
        }

        public static OverLoadModify MakeStatic(OverLoad ol, bool byStatic)
        {
            var olm = ol as OverLoadModify;
            if (olm == null)
            {
                olm = new OverLoadModify(ol);
            }
            olm.ByStatic = byStatic;
            return olm;
        }

        public override bool IsUndefined
        {
            get { return Next.IsUndefined; }
        }

        internal override Root Root
        {
            get { return Next.Root; }
        }

        public override VariantSymbol FindVariant()
        {
            return FindVariant(ByMember, ByStatic);
        }

        public override TypeSymbol FindDataType()
        {
            return FindDataType(ScopeInstance, Parameters, ByMember, ByStatic);
        }

        public override OverLoadMatch CallSelect(IReadOnlyList<TypeSymbol> args)
        {
            return CallSelect(ScopeInstance, Parameters, args, ByMember, ByStatic);
        }

        internal override IEnumerable<Scope> TraversalChilds()
        {
            return Next.TraversalChilds();
        }

        internal override IEnumerable<VariantSymbol> TraversalVariant(bool byMember, bool byStatic)
        {
            return Next.TraversalVariant(ByMember, ByStatic);
        }

        internal override IEnumerable<AttributeSymbol> TraversalAttribute()
        {
            return Next.TraversalAttribute();
        }

        internal override IEnumerable<TypeSymbol> TraversalDataType(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, bool byMember, bool byStatic)
        {
            return Next.TraversalDataType(ScopeInstance, Parameters, ByMember, ByStatic);
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args, bool byMember, bool byStatic)
        {
            return Next.TraversalCall(ScopeInstance, Parameters, args, ByMember, ByStatic);
        }

        public override string ToString()
        {
            return string.Format("Next = {{{0}}}, Parameters = !({1})", Next, Parameters.ToNames());
        }
    }
}
