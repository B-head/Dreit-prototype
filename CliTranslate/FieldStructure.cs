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
    public class FieldStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public FieldAttributes Attributes { get; private set; }
        public TypeStructure DataType { get; private set; }
        [NonSerialized]
        private FieldBuilder Builder;
        [NonSerialized]
        private FieldInfo Info;

        public FieldStructure(string name, FieldAttributes attr, TypeStructure dt, FieldInfo info = null)
        {
            Name = name;
            Attributes = attr;
            DataType = dt;
            Info = info;
        }

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateField(Name, DataType.GainType(), Attributes);
            Info = Builder;
        }

        internal override void BuildCall()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.GenerateLoad(this); //todo ストアどうしようか。
        }

        internal FieldInfo GainField()
        {
            return Info;
        }
    }
}
