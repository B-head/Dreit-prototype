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
    public class TypeStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public TypeAttributes Attributes { get; private set; }
        [NonSerialized]
        protected TypeBuilder Builder;
        [NonSerialized]
        protected Type Info;

        public TypeStructure(string name, TypeAttributes attr, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Info = info;
        }

        internal Type GainType()
        {
            RelayPreBuild();
            return Info;
        }
    }
}
