using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class DyadicOperationStructure : ExpressionStructure
    {
        public ExpressionStructure Left { get; private set; }
        public ExpressionStructure Right { get; private set; }
        public BuilderStructure Call { get; private set; }
        public ExpressionStructure Next { get; private set; }

        public DyadicOperationStructure(TypeStructure rt, ExpressionStructure left, ExpressionStructure right, BuilderStructure call, ExpressionStructure next = null)
            :base(rt)
        {
            Left = left;
            Right = right;
            Call = call;
            Next = next;
            AppendChild(Left);
            if (Next == null)
            {
                AppendChild(Right);
            }
            else
            {
                AppendChild(Next);
            }
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            Left.BuildCode();
            Right.BuildCode();
            Call.BuildCall(cg);
            if(Next != null)
            {
                Next.BuildCode();
                cg.GenerateCode(OpCodes.And);
            }
        }
    }
}
