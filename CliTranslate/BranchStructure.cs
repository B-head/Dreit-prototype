using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class BranchStructure : ExpressionStructure
    {
        public ExpressionStructure Condition { get; private set; }
        public BlockStructure Then { get; private set; }
        public BlockStructure Else { get; private set; }
        public LabelStructure ElseLabel { get; private set; }
        public LabelStructure ExitLabel { get; private set; }

        public BranchStructure(TypeStructure rt, ExpressionStructure cond, BlockStructure then, BlockStructure els)
            :base(rt)
        {
            Condition = cond;
            Then = then;
            Else = els;
            ElseLabel = new LabelStructure();
            ExitLabel = new LabelStructure();
            AppendChild(Condition);
            AppendChild(Then);
            AppendChild(Else);
            AppendChild(ElseLabel);
            AppendChild(ExitLabel);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.BeginScope();
            cg.GenerateJump(OpCodes.Brfalse, ElseLabel);
            Then.BuildCode();
            cg.GenerateJump(OpCodes.Br, ExitLabel);
            cg.MarkLabel(ElseLabel);
            Else.BuildCode();
            cg.MarkLabel(ExitLabel);
            cg.EndScope();
        }
    }
}
