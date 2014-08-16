using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    delegate IReadOnlyList<OverLoadChain> InitInherits();

    [Serializable]
    public class OverLoadChain : OverLoad
    {
        public Scope ThisScope { get; private set; }
        public OverLoadChain Parent { get; private set; }
        public IReadOnlyList<OverLoadChain> Inherits { get; private set; }
        public IReadOnlyList<OverLoadSet> Sets { get; private set; }

        internal OverLoadChain(Scope scope, OverLoadChain parent, params OverLoadSet[] sets)
        {
            if(scope == null)
            {
                throw new ArgumentNullException("current");
            }
            ThisScope = scope;
            Parent = parent;
            Inherits = new List<OverLoadChain>();
            Sets = sets;
        }

        internal OverLoadChain(Scope scope, OverLoadChain parent, IReadOnlyList<OverLoadChain> inherits, params OverLoadSet[] sets)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("current");
            }
            ThisScope = scope;
            Parent = parent;
            Inherits = inherits;
            Sets = sets;
        }

        public int TotalCountSets
        {
            get { return TraversalSets(false, false).Count(); }
        }

        public override bool IsUndefined
        {
            get { return TotalCountSets == 0; }
        }

        internal override Root Root
        {
            get { return ThisScope.Root; }
        }

        internal override IEnumerable<Scope> TraversalChilds()
        {
            foreach (var s in TraversalSets(true, true))
            {
                foreach (var v in s.TraversalChilds())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<VariantSymbol> TraversalVariant(bool byMember, bool byStatic)
        {
            foreach (var s in TraversalSets(byMember, byStatic))
            {
                foreach (var v in s.TraversalVariant(byMember, byStatic))
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<AttributeSymbol> TraversalAttribute()
        {
            foreach (var s in TraversalSets(false, true))
            {
                foreach (var v in s.TraversalAttribute())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<TypeSymbol> TraversalDataType(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, bool byMember, bool byStatic)
        {
            foreach (var s in TraversalSets(byMember, byStatic))
            {
                foreach (var v in s.TraversalDataType(inst, pars, byMember, byStatic))
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<GenericsInstance> inst,
            IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args, bool byMember, bool byStatic)
        {
            foreach (var s in TraversalSets(byMember, byStatic))
            {
                foreach (var m in s.TraversalCall(inst, pars, args, byMember, byStatic))
                {
                    yield return m;
                }
            }
        }

        internal IEnumerable<OverLoadSet> TraversalSets(bool byMember, bool byStatic)
        {
            foreach(var s in Sets)
            {
                yield return s;
            }
            if (!byStatic)
            {
                foreach (var i in Inherits)
                {
                    foreach (var s in i.TraversalSets(true, false))
                    {
                        yield return s;
                    }
                }
            }
            if (!byMember && Parent != null)
            {
                foreach (var s in Parent.TraversalSets(false, byStatic))
                {
                    yield return s;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("TotalSets = {0}, This = {{{1}}}", TotalCountSets, ThisScope);
        }
    }
}
