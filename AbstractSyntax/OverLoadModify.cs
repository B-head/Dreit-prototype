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
