using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class TextStructure : CilStructure
    {
        public string Value { get; private set; }

        public TextStructure(string value)
        {
            Value = value;
        }
    }
}
