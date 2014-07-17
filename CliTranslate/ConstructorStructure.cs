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

        public ConstructorStructure(MethodAttributes attr, IReadOnlyList<ParameterStructure> arg, ConstructorInfo info = null)
            :base(attr, arg)
        {
            Info = info;
        }

        protected override void BuildCode()
        {
            if (Info != null)
            {
                return;
            }
            var cont = (ContainerStructure)Parent;
            Builder = cont.CreateConstructor(Attributes, Arguments.ToTypes());
            Info = Builder;
            Arguments.RegisterBuilders(Builder);
        }
    }
}
