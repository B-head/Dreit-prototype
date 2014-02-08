using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CliTranslate
{
    public class EnumTranslator : Translator
    {
        public Type TypeInfo { get; private set; }

        public EnumTranslator(FullPath path, Translator parent, Type type = null)
            : base(path, parent)
        {
            TypeInfo = type;
            if(type != null)
            {
                //保留
            }
        }
    }
}
