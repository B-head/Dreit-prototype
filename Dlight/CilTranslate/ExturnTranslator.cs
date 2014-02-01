using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class ExturnTranslator : Translator
    {
        private Type EmbedType { get; set; }

        public ExturnTranslator(Type type, Translator parent)
            : base(type.Name, parent)
        {
            EmbedType = type;
        }
    }
}
