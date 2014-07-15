using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class GotoStructure : CilStructure
    {
        public LabelStructure Label { get; private set; }

        public GotoStructure(LabelStructure label)
        {
            Label = label;
            AppendChild(Label);
        }
    }
}
