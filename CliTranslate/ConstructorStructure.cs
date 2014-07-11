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
    public class ConstructorStructure : MethodBaseStructure
    {
        public ConstructorBuilder Builder { get; private set; }

        public ConstructorStructure(MethodAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, ConstructorBuilder bld = null)
            :base(attr, gnr, arg, ret)
        {
            Builder = bld;
        }
    }
}
