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
using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ArgumentSymbol : VariantSymbol
    {
        public ArgumentSymbol()
        {

        }

        protected ArgumentSymbol(TextPosition tp, VariantType type, Element def)
            : base(tp, type, def)
        {
        }

        public new void Initialize(string name, VariantType type, IReadOnlyList<AttributeSymbol> attr, TypeSymbol dt, Element def)
        {
            base.Initialize(name, type, attr, dt, def);
        }

        internal static IReadOnlyList<ArgumentSymbol> MakeParameters(params TypeSymbol[] types)
        {
            var ret = new List<ArgumentSymbol>();
            for (var i = 0; i < types.Length; ++i)
            {
                var p = new ArgumentSymbol();
                p.Initialize("@@arg" + (i + 1), VariantType.Let, new List<AttributeSymbol>(), types[i]);
                ret.Add(p);
            }
            return ret;
        }

        internal static bool HasVariadic(IReadOnlyList<Scope> f)
        {
            if (f.Count == 0)
            {
                return false;
            }
            return f.Last().Attribute.HasAnyAttribute(AttributeType.Variadic);
        }
    }
}
