using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class MonadicOperationStructure : CilStructure
    {
        public CilStructure Expression { get; private set; }
        public CilStructure Call { get; private set; }

        public MonadicOperationStructure(CilStructure exp, CilStructure call)
        {
            Expression = exp;
            Call = call;
        }
    }
}
