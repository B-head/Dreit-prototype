using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class StringStructure : CilStructure
    {
        public IReadOnlyList<CilStructure> Expressions { get; private set; }

        public StringStructure(IReadOnlyList<CilStructure> exps)
        {
            Expressions = exps;
            AppendChild(Expressions);
        }
    }
}
