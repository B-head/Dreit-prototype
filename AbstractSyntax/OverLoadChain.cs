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
        public OverLoadChain Next { get; private set; }
        private IReadOnlyList<OverLoadChain> _Inherits;
        [NonSerialized] private InitInherits InitInherits;
        public IReadOnlyList<OverLoadSet> Sets { get; private set; }

        internal OverLoadChain(Scope scope, OverLoadChain next, params OverLoadSet[] sets)
        {
            if(scope == null)
            {
                throw new ArgumentNullException("current");
            }
            ThisScope = scope;
            Next = next;
            InitInherits = () => new List<OverLoadChain>();
            Sets = sets;
        }

        internal OverLoadChain(Scope scope, OverLoadChain next, InitInherits initInherits, params OverLoadSet[] sets)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("current");
            }
            ThisScope = scope;
            Next = next;
            InitInherits = initInherits;
            Sets = sets;
        }

        public IReadOnlyList<OverLoadChain> Inherits
        {
            get
            {
                if (_Inherits == null)
                {
                    _Inherits = InitInherits();
                }
                return _Inherits;
            }
        }

        public override bool IsUndefined
        {
            get { return TraversalSets(true, true).Count() == 0; }
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

        internal override IEnumerable<VariantSymbol> TraversalVariant()
        {
            foreach (var s in TraversalSets(true, false))
            {
                foreach (var v in s.TraversalVariant())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<Scope> TraversalDataType()
        {
            foreach (var s in TraversalSets(true, false))
            {
                foreach (var v in s.TraversalDataType())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            foreach (var s in TraversalSets(true, true))
            {
                foreach (var m in s.TraversalCall(pars, args))
                {
                    yield return m;
                }
            }
        }

        internal IEnumerable<OverLoadSet> TraversalSets(bool byNext, bool byInherit)
        {
            foreach(var s in Sets)
            {
                yield return s;
            }
            if (byInherit)
            {
                foreach (var i in Inherits)
                {
                    foreach (var s in i.TraversalSets(false, true))
                    {
                        yield return s;
                    }
                }
            }
            if (byNext && Next != null)
            {
                foreach (var s in Next.TraversalSets(true, byInherit))
                {
                    yield return s;
                }
            }
        }
    }
}
