using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CallStructure : CilStructure
    {
        public CilStructure Call { get; private set; }
        public CilStructure Pre { get; private set; }
        public IReadOnlyList<CilStructure> Arguments { get; private set; }

        public CallStructure(CilStructure call, CilStructure pre, IReadOnlyList<CilStructure> args)
        {
            Call = call;
            Pre = pre;
            Arguments = args;
            AppendChild(Call);
            AppendChild(Pre);
            AppendChild(Arguments);
        }
    }
}
