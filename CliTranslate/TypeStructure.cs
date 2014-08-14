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

        internal override bool IsDataTypeContext
        {
            get { return true; }
        }

        public void Initialize(string name, TypeAttributes attr, BlockStructure block = null, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Block = block;
            AppendChild(Block);
            Info = info;
        }

        internal virtual Type GainType()
        {
            RelayPreBuild();
            return Info;
        }

        internal bool IsReferType
        {
            get { return Info.IsClass || Info.IsInterface; }
        }

        internal bool IsValueType
        {
            get { return Info.IsValueType; }
        }

        internal bool IsVoid
        {
            get { return Info == typeof(void); }
        }
    }
}
