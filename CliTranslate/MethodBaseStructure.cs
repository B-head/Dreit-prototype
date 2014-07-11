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
    public abstract class MethodBaseStructure : ContainerStructure
    {
        public MethodAttributes Attributes { get; private set; }
        public IReadOnlyList<GenericTypeParameterStructure> Generics { get; private set; }
        public IReadOnlyList<ParameterStructure> Arguments { get; private set; }
        public TypeStructure ReturnType { get; private set; }

        protected MethodBaseStructure(MethodAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret)
        {
            Attributes = attr;
            Generics = gnr;
            Arguments = arg;
            ReturnType = ret;
        }
    }
}
