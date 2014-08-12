using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoopParameterStructure : ExpressionStructure
    {
        public string Name { get; private set; }
        public TypeStructure DataType { get; private set; }
        public CilStructure DefaultValue { get; private set; }
        public LocalStructure Local { get; private set; }

        public LoopParameterStructure(string name, TypeStructure dt, CilStructure def)
            :base(dt)
        {
            Name = name;
            DataType = dt;
            DefaultValue = def;
            Local = new LocalStructure(name, dt);
            AppendChild(DefaultValue);
            AppendChild(Local);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            if(DefaultValue == null)
            {
                cg.GenerateLoad(Local);
            }
            else
            {
                DefaultValue.BuildCode();
                cg.GenerateStore(Local);
                cg.GenerateLoad(Local);
            }
        }
    }
}
