using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ValueStructure : ExpressionStructure
    {
        public dynamic Value { get; private set; }

        public ValueStructure(TypeStructure rt, dynamic value)
            :base(rt)
        {
            Value = value;
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.GeneratePrimitive(Value);
        }
    }
}
