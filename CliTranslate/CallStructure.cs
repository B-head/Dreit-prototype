using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CallStructure : ExpressionStructure
    {
        public BuilderStructure Call { get; private set; }
        public ExpressionStructure Pre { get; private set; }
        public CilStructure Access { get; private set; }
        public IReadOnlyList<ExpressionStructure> Arguments { get; private set; }

        public CallStructure(TypeStructure rt, BuilderStructure call, ExpressionStructure pre)
            : base(rt)
        {
            Call = call;
            Pre = pre;
            Access = pre;
            Arguments = new List<ExpressionStructure>();
            if (Access != null)
            {
                AppendChild(Access);
            }
            AppendChild(Arguments);
        }

        public CallStructure(TypeStructure rt, BuilderStructure call, ExpressionStructure pre, CilStructure access, IReadOnlyList<ExpressionStructure> args)
            :base(rt)
        {
            Call = call;
            Pre = pre;
            Access = access;
            Arguments = args;
            if (Access != null)
            {
                AppendChild(Access);
            }
            AppendChild(Arguments);
        }

        internal override void BuildCode()
        {
            if(CurrentContainer.IsDataTypeContext)
            {
                return;
            }
            var cg = CurrentContainer.GainGenerator();
            if (Pre != null)
            {
                Pre.BuildCode();
            }
            foreach (var v in Arguments)
            {
                v.BuildCode();
            }
            Call.BuildCall(cg);
        }
    }
}
