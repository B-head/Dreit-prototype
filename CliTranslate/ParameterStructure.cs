using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class ParameterStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public ParameterAttributes Attributes { get; private set; }
        public TypeStructure ParamType { get; private set; }
        public CilStructure DefaultValue { get; private set; }
        [NonSerialized]
        private ParameterBuilder Builder;

        public ParameterStructure(string name, ParameterAttributes attr, TypeStructure pt, CilStructure def)
        {
            Name = name;
            Attributes = attr;
            ParamType = pt;
            DefaultValue = def;
            AppendChild(DefaultValue);
        }

        internal void RegisterBuilder(ParameterBuilder builder)
        {
            if(Builder != null)
            {
                throw new InvalidOperationException();
            }
            Builder = builder;
        }

        internal override void BuildCall()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.GenerateLoad(this); //todo ストアどうしようか。
        }

        internal int GainPosition()
        {
            return Builder.Position;
        }
    }
}
