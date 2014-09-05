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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    public enum ModifyType
    {
        Unknown,
        Refer,
        Typeof,
        Nullable,
        Pointer,
        EmbedArray,
    }

    [Serializable]
    public class ModifyTypeSymbol : TypeSymbol
    {
        public ModifyType ModifyType { get; private set; }
        private IReadOnlyList<GenericSymbol> _Generics;
        private IReadOnlyList<TypeSymbol> _Inherit;

        public ModifyTypeSymbol(ModifyType type)
        {
            Name = "@@" + type.ToString();
            ModifyType = type;
            var g = new GenericSymbol("T", new List<AttributeSymbol>(), new List<Scope>());
            _Generics = new GenericSymbol[] { g };
            _Inherit = new TypeSymbol[] { g };
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get { return _Generics; }
        }

        public override IReadOnlyList<TypeSymbol> Inherit
        {
            get { return _Inherit; }
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }

        public static bool HasContainModify(Scope type, ModifyType modify)
        {
            var t = type as ClassTemplateInstance;
            if (t == null)
            {
                return false;
            }
            var m = t.Type as ModifyTypeSymbol;
            if (m == null)
            {
                return false;
            }
            return m.ModifyType == modify;
        }

        public static bool HasInheritModify(ModifyType type)
        {
            return type == ModifyType.Refer ||
                 type == ModifyType.Typeof ||
                 type == ModifyType.Nullable ||
                 type == ModifyType.Pointer;
        }
    }
}
