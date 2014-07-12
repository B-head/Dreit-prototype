using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class NumberStructure : CilStructure
    {
        public dynamic Value { get; private set; }

        public NumberStructure(dynamic value)
        {
            Value = value;
        }
    }
}
