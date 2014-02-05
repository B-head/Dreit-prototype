using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    class EnumTranslator : Translator
    {
        public Type TypeInfo { get; private set; }

        public EnumTranslator(string name, Translator parent, Type type = null)
            : base(name, parent)
        {
            TypeInfo = type;
            if(type != null)
            {
                //保留
            }
        }
    }
}
