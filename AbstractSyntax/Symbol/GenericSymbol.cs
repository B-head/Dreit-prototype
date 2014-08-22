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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class GenericSymbol : TypeSymbol
    {
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected IReadOnlyList<Scope> _Constraint;

        protected GenericSymbol(TextPosition tp, string name)
            :base(tp)
        {
            Name = name;
        }

        public GenericSymbol(string name, IReadOnlyList<AttributeSymbol> attr, IReadOnlyList<Scope> constraint)
        {
            Name = name;
            _Attribute = attr;
            _Constraint = constraint;
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }
    }
}
