using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class PolyTranslator : Translator
    {
        public PolyTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }
    }
}
