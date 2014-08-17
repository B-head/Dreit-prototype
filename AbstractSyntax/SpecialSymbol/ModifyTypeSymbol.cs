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

        public ModifyTypeSymbol(ModifyType type)
            
        {
            ModifyType = type;
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
