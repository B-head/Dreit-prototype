using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ModifyTypeStructure : TypeStructure
    {
        public ModifyType ModifyType { get; private set; }

        public ModifyTypeStructure(ModifyType type)
        {
            ModifyType = type;
        }
    }
}
