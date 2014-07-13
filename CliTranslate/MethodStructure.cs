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

        public MethodStructure(string name, MethodAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, MethodBuilder bld = null)
            :base(attr, gnr, arg, ret)
        {
            Name = name;
            Builder = bld;
        }
    }
}
