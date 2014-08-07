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

        public void Initialize(bool isInstance, MethodAttributes attr, IReadOnlyList<ParameterStructure> arg, BlockStructure block = null, ConstructorInfo info = null)
        {
            Info = info;
            base.Initialize(isInstance, attr, arg, block);
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
            Arguments.RegisterBuilders(Builder, IsInstance);
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
