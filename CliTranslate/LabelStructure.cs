using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LabelStructure : BuilderStructure
    {
        [NonSerialized]
        private Label Label;

        internal Label GainLabel()
        {
            return Label;
        }

        protected override void PreBuild()
        {
            var cg = CurrentContainer.GainGenerator();
            Label = cg.CreateLabel();
        }
    }
}
