using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public abstract class ExpressionStructure : CilStructure
    {
        public TypeStructure ResultType { get; private set; }

        public ExpressionStructure(TypeStructure rt)
        {
            ResultType = rt;
        }
    }
}
