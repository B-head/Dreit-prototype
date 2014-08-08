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
    public class LocalStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public TypeStructure DataType { get; private set; }
        [NonSerialized]
        private LocalBuilder Builder;

        public LocalStructure(TypeStructure dt)
        {
            DataType = dt;
        }

        public LocalStructure(string name, TypeStructure dt)
        {
            Name = name;
            DataType = dt;
        }

        public LocalStructure(TypeStructure dt, CodeGenerator cg)
        {
            DataType = dt;
            Builder = cg.CreateLocal(DataType);
        }

        protected override void PreBuild()
        {
            var cg = CurrentContainer.GainGenerator();
            Builder = cg.CreateLocal(DataType);
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Builder.SetLocalSymInfo(Name);
            }
        }

        internal LocalBuilder GainLocal()
        {
            return Builder;
        }
    }
}
