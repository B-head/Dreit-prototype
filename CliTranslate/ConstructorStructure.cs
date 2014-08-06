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

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateConstructor(Attributes, Arguments.ToTypes());
            Info = Builder;
            Arguments.RegisterBuilders(Builder);
            SpreadGenerator();
        }

        internal override void PostBuild()
        {
            Generator.GenerateControl(OpCodes.Ret);
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            cg.GenerateNew(this);
        }

        protected override ILGenerator GainILGenerator()
        {
            return Builder.GetILGenerator();
        }

        internal ConstructorInfo GainConstructor()
        {
            return Info;
        }
    }
}
