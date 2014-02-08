using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CliTranslate
{
    public class ArgumentTranslator : Translator
    {
        public ArgumentTranslator(FullPath path, Translator parent)
            : base(path, parent)
        {

        }
    }
}
