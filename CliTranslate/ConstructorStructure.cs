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
        [NonSerialized]
        private ConstructorBuilder Builder;
        [NonSerialized]
        private ConstructorInfo Info;

        public ConstructorStructure(MethodAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, ConstructorInfo info = null)
            :base(attr, gnr, arg, ret)
        {
            Info = info;
        }
    }
}
