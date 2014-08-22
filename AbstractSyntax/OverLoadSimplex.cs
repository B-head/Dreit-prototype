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

        internal override IEnumerable<OverLoadTypeMatch> TraversalDataType(IReadOnlyList<TypeSymbol> pars)
        {
            var type = Symbol as TypeSymbol;
            if (type != null)
            {
                var inst = new List<GenericsInstance>();
                yield return OverLoadTypeMatch.MakeMatch(Root, type, type.Generics, inst, pars);
            }
        }

        internal override IEnumerable<OverLoadCallMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
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
