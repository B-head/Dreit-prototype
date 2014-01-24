using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class EmbedTypeTranslator : CilTranslator
    {
        private Type EmbedType { get; set; }

        public EmbedTypeTranslator(string name, Type type)
            :base(name)
        {
            EmbedType = type;
        }

        public override Type GetDataType()
        {
            return EmbedType;
        }
    }
}
