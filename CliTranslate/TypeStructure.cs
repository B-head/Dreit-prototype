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
    public abstract class TypeStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public TypeAttributes Attributes { get; private set; }
        public BlockStructure Block { get; private set; }
        [NonSerialized]
        protected Type Info;

        protected TypeStructure()
        {
        }

        public void Initialize(string name, TypeAttributes attr, BlockStructure block = null, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Block = block;
            AppendChild(Block);
            Info = info;
        }

        internal Type GainType()
        {
            RelayPreBuild();
            return Info;
        }
    }
}
