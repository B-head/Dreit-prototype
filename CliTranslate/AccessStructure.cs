using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class AccessStructure : ExpressionStructure
    {
        public BuilderStructure Call { get; private set; }
        public ExpressionStructure Pre { get; private set; }

        public AccessStructure(TypeStructure rt, BuilderStructure call, ExpressionStructure pre = null)
            :base(rt)
        {
            Call = call;
            Pre = pre;
            AppendChild(Pre);
        }

        internal override void BuildCode()
        {
            if (Pre != null)
            {
                Pre.BuildCode();
            }
            Call.BuildCall();
        }
    }
}
