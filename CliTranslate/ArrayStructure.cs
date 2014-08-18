using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ArrayStructure : ExpressionStructure
    {
        public ArrayStructure(TypeStructure rt, IReadOnlyList<ExpressionStructure> exps)
            :base(rt)
        {
            AppendChild(exps);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.GenerateList(ResultType, this);
        }

    }
}
