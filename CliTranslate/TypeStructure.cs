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
        public TypeAttributes Attributes { get; private set; }
        public IReadOnlyList<GenericTypeParameterStructure> Generics { get; private set; }
        public TypeStructure BaseType { get; private set; }
        public IReadOnlyList<TypeStructure> Implements { get; private set; }
        public TypeBuilder Builder { get; private set; }

        public TypeStructure(TypeAttributes attr, IReadOnlyList<GenericTypeParameterStructure> gnr, TypeStructure bt, IReadOnlyList<TypeStructure> imp, TypeBuilder bld = null)
        {
            Attributes = attr;
            Generics = gnr;
            BaseType = bt;
            Implements = imp;
            Builder = bld;
        }
    }
}
