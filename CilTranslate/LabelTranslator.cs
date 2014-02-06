using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CilTranslate
{
    public class LabelTranslator : Translator
    {
        public LabelTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }
    }
}
