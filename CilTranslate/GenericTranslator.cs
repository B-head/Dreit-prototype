using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CilTranslate
{
    public class GenericTranslator : Translator
    {
        public GenericTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }
    }
}
