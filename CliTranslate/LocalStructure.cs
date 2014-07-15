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
    public class LocalStructure : CilStructure
    {
        public string Name { get; private set; }
        public TypeStructure DataType { get; private set; }
        [NonSerialized]
        private LocalBuilder Builder;

        public LocalStructure(TypeStructure dt)
        {
            DataType = dt;
            AppendChild(DataType);
        }

        public LocalStructure(string name, TypeStructure dt)
        {
            Name = name;
            DataType = dt;
            AppendChild(DataType);
        }
    }
}
