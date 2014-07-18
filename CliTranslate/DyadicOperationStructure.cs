using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public DyadicOperationStructure(TypeStructure rt, ExpressionStructure left, ExpressionStructure right, BuilderStructure call)
            :base(rt)
        {
            Left = left;
            Right = right;
            Call = call;
            AppendChild(Left);
            AppendChild(Right);
        }

        internal override void BuildCode()
        {
            Left.BuildCode();
            Right.BuildCode();
            Call.BuildCall();
        }
    }
}
