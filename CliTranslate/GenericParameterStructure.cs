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
    public class GenericParameterStructure : TypeStructure
    {
        public GenericParameterAttributes GenericAttributes { get; private set; }
        public IReadOnlyList<CilStructure> Constraints { get; private set; }
        [NonSerialized]
        private GenericTypeParameterBuilder Builder;

        public GenericParameterStructure(string name, GenericParameterAttributes attr, IReadOnlyList<CilStructure> constant, Type info = null)
        {
            GenericAttributes = attr;
            Constraints = constant;
            Info = info;
            base.Initialize(name, 0);
        }

        internal void RegisterBuilder(GenericTypeParameterBuilder builder)
        {
            if (Builder != null)
            {
                throw new InvalidOperationException();
            }
            Builder = builder;
            Info = Builder;
        }
    }
}
