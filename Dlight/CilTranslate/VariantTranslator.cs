using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class VariantTranslator : Translator
    {
        public VariantTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }
    }
}
