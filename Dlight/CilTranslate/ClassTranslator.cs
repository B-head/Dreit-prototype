using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    class ClassTranslator : ContextTranslator
    {
        public ClassTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }
    }
}
