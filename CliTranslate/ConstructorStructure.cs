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
        public bool IsDefault { get; private set; }
        public ConstructorStructure SuperConstructor { get; private set; }
        [NonSerialized]
        private ConstructorBuilder Builder;
        [NonSerialized]
        private ConstructorInfo Info;

        public void Initialize(MethodAttributes attr, IReadOnlyList<ParameterStructure> arg, BlockStructure block, ConstructorInfo info = null)
        {
            Info = info;
            base.Initialize(true, attr, arg, block);
        }

        public void InitializeDefault()
        {
            IsDefault = true;
            var arg = new List<ParameterStructure>();
            base.Initialize(true, MethodAttributes.Public, arg, null);
        }

        public void RegisterSuperConstructor(ConstructorStructure super)
        {
            SuperConstructor = super;
        }

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var pt = CurrentContainer as PureTypeStructure;
            Builder = pt.CreateConstructor(Attributes, Arguments.ToTypes());
            Info = Builder;
            Arguments.RegisterBuilders(Builder, IsInstance);
            SpreadGenerator();
            foreach(var f in pt.GetFields())
            {
                f.BuildInitValue(Generator);
            }
            if(SuperConstructor != null)
            {
                Generator.GenerateControl(OpCodes.Ldarg_0);
                Generator.GenerateCall(SuperConstructor);
            }
        }

        internal override void PostBuild()
        {
            if(Generator == null)
            {
                return;
            }
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

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            var ret = new ConstructorStructure();
            ret.Info = type.RenewConstructor(this);
            return ret;
        }
    }
}
