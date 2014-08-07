using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class BlockStructure : ExpressionStructure
    {
        public BlockStructure(TypeStructure rt, IReadOnlyList<CilStructure> exps)
            :base(rt)
        {
            AppendChild(exps);
        }
    }
}
