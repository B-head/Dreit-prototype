using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LogicalStructure : ExpressionStructure
    {
        public ExpressionStructure Left { get; private set; }
        public ExpressionStructure Right { get; private set; }
        public bool IsOr { get; private set;}
        public LabelStructure Exit { get; private set; }

        public LogicalStructure(TypeStructure rt, ExpressionStructure left, ExpressionStructure right, bool isOr)
            :base(rt)
        {
            Left = left;
            Right = right;
            IsOr = isOr;
            Exit = new LabelStructure();
            AppendChild(Left);
            AppendChild(Right);
            AppendChild(Exit);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            if (IsOr)
            {
                Left.BuildCode();
                cg.GenerateCode(OpCodes.Dup);
                cg.GenerateJump(OpCodes.Brtrue, Exit);
                cg.GenerateCode(OpCodes.Pop);
                Right.BuildCode();
                cg.MarkLabel(Exit);
            }
            else
            {
                Left.BuildCode();
                cg.GenerateCode(OpCodes.Dup);
                cg.GenerateJump(OpCodes.Brfalse, Exit);
                cg.GenerateCode(OpCodes.Pop);
                Right.BuildCode();
                cg.MarkLabel(Exit);
            }
        }
    }
}
