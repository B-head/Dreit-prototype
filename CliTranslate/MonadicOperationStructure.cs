using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class MonadicOperationStructure : ExpressionStructure
    {
        public ExpressionStructure Expression { get; private set; }
        public BuilderStructure Call { get; private set; }

        public MonadicOperationStructure(TypeStructure rt, ExpressionStructure exp, BuilderStructure call)
            :base(rt)
        {
            Expression = exp;
            Call = call;
            AppendChild(Expression);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            Expression.BuildCode();
            Call.BuildCall(cg);
        }
    }
}
