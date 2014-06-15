using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class BranchTranslator : Translator
    {
        internal Label EndLabel { get; private set; }
        internal Label ElseLabel { get; private set; }

        public BranchTranslator(IScope path, Translator parent, bool definedElse = false)
            : base(path, parent)
        {
            Generator = parent.Generator;
            EndLabel = CreateLabel();
            ElseLabel = (definedElse ? CreateLabel() : EndLabel);
        }


    }
}
