using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.Translate
{
    class EmbedTypeTranslator : Translator
    {
        private Type EmbedType { get; set; }

        public EmbedTypeTranslator(FullName fullname, Translator parent, Type type)
            : base(fullname, parent)
        {
            EmbedType = type;
        }

        public override Type GetDataType()
        {
            return EmbedType;
        }
    }
}
