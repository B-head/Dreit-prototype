using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoopStructure : ExpressionStructure
    {
        public ExpressionStructure Condition { get; private set; }
        public ExpressionStructure On { get; private set; }
        public ExpressionStructure By { get; private set; }
        public BlockStructure Block { get; private set; }
        public LabelStructure BreakLabel { get; private set; }
        public LabelStructure ContinueLabel { get; private set; }
        public LabelStructure PlungeLabel { get; private set; }

        public LoopStructure(TypeStructure rt, ExpressionStructure cond, ExpressionStructure on, ExpressionStructure by, BlockStructure block)
            :base(rt)
        {
            Condition = cond;
            On = on;
            By = by;
            Block = block;
            BreakLabel = new LabelStructure();
            ContinueLabel = new LabelStructure();
            PlungeLabel = new LabelStructure();
            AppendChild(Condition);
            AppendChild(On);
            AppendChild(By);
            AppendChild(Block);
            AppendChild(BreakLabel);
            AppendChild(ContinueLabel);
            AppendChild(PlungeLabel);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.BeginScope();
            PopBuildCode(On);
            cg.GenerateJump(OpCodes.Br, PlungeLabel);
            cg.MarkLabel(ContinueLabel);
            PopBuildCode(By);
            cg.MarkLabel(PlungeLabel);
            Condition.BuildCode();
            cg.GenerateJump(OpCodes.Brfalse, BreakLabel);
            PopBuildCode(Block);
            cg.GenerateJump(OpCodes.Br, ContinueLabel);
            cg.MarkLabel(BreakLabel);
            cg.EndScope();
        }
    }
}
