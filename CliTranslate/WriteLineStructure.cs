using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class WriteLineStructure : ExpressionStructure
    {
        public ExpressionStructure Expression { get; private set; }

        public WriteLineStructure(TypeStructure rt, ExpressionStructure exp)
            :base(rt)
        {
            Expression = exp;
            AppendChild(Expression);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.GenerateWriteLine(Expression);
        }
    }
}
