using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CacheStructure : CilStructure
    {
        public CilStructure Expression { get; private set; }
        public LocalStructure Cache { get; private set; }

        public CacheStructure(CilStructure exp)
        {
            Expression = exp;
            Cache = new LocalStructure();
        }
    }
}
