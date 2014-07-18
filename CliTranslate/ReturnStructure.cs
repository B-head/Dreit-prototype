using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ReturnStructure : ExpressionStructure
    {
        public ExpressionStructure Expression { get; private set; }

        public ReturnStructure(TypeStructure rt, ExpressionStructure exp)
            :base(rt)
        {
            Expression = exp;
            AppendChild(Expression);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            Expression.BuildCode();
            cg.GenerateControl(OpCodes.Ret);
        }
    }
}
