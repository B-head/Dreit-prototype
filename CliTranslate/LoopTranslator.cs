using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class LoopTranslator : BranchTranslator
    {
        internal Label StartLabel { get; private set; }

        public LoopTranslator(IScope path, Translator parent)
            : base(path, parent, false)
        {
            Generator = parent.Generator;
            StartLabel = CreateLabel();
        }

        public override Label GetContinueLabel()
        {
            return StartLabel;
        }

        public override Label GetBreakLabel()
        {
            return EndLabel;
        }
    }
}
