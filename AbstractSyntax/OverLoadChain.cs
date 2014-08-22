/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
            get { return TraversalSets().Count(); }
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
            foreach (var s in TraversalSets())
            {
                foreach (var v in s.TraversalChilds())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<VariantSymbol> TraversalVariant()
        {
            foreach (var s in TraversalSets())
            {
                foreach (var v in s.TraversalVariant())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<AttributeSymbol> TraversalAttribute()
        {
            foreach (var s in TraversalSets())
            {
                foreach (var v in s.TraversalAttribute())
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<OverLoadTypeMatch> TraversalDataType(IReadOnlyList<TypeSymbol> pars)
        {
            foreach (var s in TraversalSets())
            {
                foreach (var v in s.TraversalDataType(pars))
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<OverLoadCallMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            foreach (var s in TraversalSets())
            {
                foreach (var m in s.TraversalCall(pars, args))
                {
                    yield return m;
                }
            }
        }

        internal IEnumerable<OverLoadSet> TraversalSets(bool byMember = false)
        {
            foreach (var s in Sets)
            {
                yield return s;
            }
            foreach (var i in Inherits)
            {
                foreach (var s in i.TraversalSets(true))
                {
                    yield return s;
                }
            }
            if (!byMember && Parent != null)
            {
                foreach (var s in Parent.TraversalSets(false))
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
