using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class DyadicOperationStructure : CilStructure
    {
        public CilStructure Left { get; private set; }
        public CilStructure Right { get; private set; }
        public CilStructure Call { get; private set; }

        public DyadicOperationStructure(CilStructure left, CilStructure right, CilStructure call)
        {
            Left = left;
            Right = right;
            Call = call;
        }
    }
}
