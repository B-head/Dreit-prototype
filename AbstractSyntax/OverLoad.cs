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
    public abstract class OverLoad
    {
        public VariantSymbol FindVariant()
        {
            foreach (var v in TraversalVariant())
            {
                return v;
            }
            return Root.ErrorVariant;
        }

        public AttributeSymbol FindAttribute()
        {
            foreach (var v in TraversalAttribute())
            {
                return v;
            }
            throw new InvalidOperationException();
        }

        public OverLoadTypeMatch FindDataType()
        {
            var pars = new List<TypeSymbol>();
            foreach (var v in TraversalDataType(pars))
            {
                return v; //todo 型の選択をする。
            }
            return OverLoadTypeMatch.MakeNotType(Root.ErrorType);
        }

        public OverLoadCallMatch CallSelect()
        {
            return CallSelect(new List<TypeSymbol>());
        }

        public OverLoadCallMatch CallSelect(IReadOnlyList<TypeSymbol> args)
        {
            if (TypeSymbol.HasAnyErrorType(args))
            {
                return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
            }
            var result = OverLoadCallMatch.MakeNotCallable(Root.ErrorRoutine);
            var pars = new List<TypeSymbol>();
            foreach (var m in TraversalCall(pars, args))
            {
                var a = OverLoadCallMatch.GetMatchPriority(result.Result);
                var b = OverLoadCallMatch.GetMatchPriority(m.Result);
                if (a < b)
                {
                    result = m; //todo 優先順位が重複した場合の対処が必要。
                }
            }
            return result;
        }

        public abstract bool IsUndefined { get; }
        internal abstract Root Root { get; }
        internal abstract IEnumerable<Scope> TraversalChilds();
        internal abstract IEnumerable<VariantSymbol> TraversalVariant();
        internal abstract IEnumerable<AttributeSymbol> TraversalAttribute();
        internal abstract IEnumerable<OverLoadTypeMatch> TraversalDataType(IReadOnlyList<TypeSymbol> pars);
        internal abstract IEnumerable<OverLoadCallMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args);
    }
}
