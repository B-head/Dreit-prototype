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
    public class MethodStructure : MethodBaseStructure
    {
        public string Name { get; private set; } 
        [NonSerialized]
        private MethodBuilder Builder;
        [NonSerialized]
        private MethodInfo Info;

        public MethodStructure(string name, MethodAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, MethodInfo info = null)
            :base(attr, gnr, arg, ret)
        {
            Name = name;
            Info = info;
        }
    }
}
