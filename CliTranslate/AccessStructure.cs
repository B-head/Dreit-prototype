using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class AccessStructure : CilStructure
    {
        public CilStructure Call { get; private set; }
        public CilStructure Pre { get; private set; }

        public AccessStructure(CilStructure call, CallStructure pre = null)
        {
            Call = call;
            Pre = pre;
            AppendChild(Pre);
        }
    }
}
