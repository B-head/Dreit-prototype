using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ReturnStructure : CilStructure
    {
        public CilStructure Expression { get; private set; }

        public ReturnStructure(CilStructure exp)
        {
            Expression = exp;
        }
    }
}
