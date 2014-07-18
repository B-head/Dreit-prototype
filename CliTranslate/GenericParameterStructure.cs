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
    public class GenericParameterStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public GenericParameterAttributes Attributes { get; private set; }
        public IReadOnlyList<CilStructure> Constraints { get; private set; }
        [NonSerialized]
        private GenericTypeParameterBuilder Builder;
        [NonSerialized]
        private Type Info;

        public GenericParameterStructure(string name, GenericParameterAttributes attr, IReadOnlyList<CilStructure> constant, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Constraints = constant;
            Info = info;
        }

        internal void RegisterBuilder(GenericTypeParameterBuilder builder)
        {
            if (Builder != null)
            {
                throw new InvalidOperationException();
            }
            Builder = builder;
        }
    }
}
