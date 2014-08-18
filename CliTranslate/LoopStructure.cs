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
        public ExpressionStructure Use { get; private set; }
        public ExpressionStructure By { get; private set; }
        public BlockStructure Block { get; private set; }
        public LabelStructure BreakLabel { get; private set; }
        public LabelStructure ContinueLabel { get; private set; }
        public LabelStructure PlungeLabel { get; private set; }

        public LoopStructure(TypeStructure rt)
            :base(rt)
        {
            BreakLabel = new LabelStructure();
            ContinueLabel = new LabelStructure();
            PlungeLabel = new LabelStructure();
            AppendChild(BreakLabel);
            AppendChild(ContinueLabel);
            AppendChild(PlungeLabel);
        }

        public void Initialize(ExpressionStructure cond, ExpressionStructure use, ExpressionStructure by, BlockStructure block)
        {
            Condition = cond;
            Use = use;
            By = by;
            Block = block;
            AppendChild(Condition);
            AppendChild(Use);
            AppendChild(By);
            AppendChild(Block);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.BeginScope();
            if (Use != null)
            {
                PopBuildCode(Use);
            }
            cg.GenerateJump(OpCodes.Br, PlungeLabel);
            cg.MarkLabel(ContinueLabel);
            if (By != null)
            {
                PopBuildCode(By);
            }
            cg.MarkLabel(PlungeLabel);
            if (Condition != null)
            {
                Condition.BuildCode();
                cg.GenerateJump(OpCodes.Brfalse, BreakLabel);
            }
            Block.BuildCode();
            cg.GenerateJump(OpCodes.Br, ContinueLabel);
            cg.MarkLabel(BreakLabel);
            cg.EndScope();
        }
    }
}
