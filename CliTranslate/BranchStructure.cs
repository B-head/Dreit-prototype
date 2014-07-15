using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class BranchStructure : CilStructure
    {
        public CilStructure Condition { get; private set; }
        public LabelStructure Jump { get; private set; }

        public BranchStructure(CilStructure cond, LabelStructure jump)
        {
            Condition = cond;
            Jump = jump;
            AppendChild(Condition);
            AppendChild(Jump);
        }
    }
}
