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
