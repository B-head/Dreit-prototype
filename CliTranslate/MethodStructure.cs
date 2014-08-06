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
    public class MethodStructure : MethodBaseStructure
    {
        public string Name { get; private set; }
        public IReadOnlyList<GenericParameterStructure> Generics { get; private set; }
        public TypeStructure ReturnType { get; private set; }
        [NonSerialized]
        private MethodBuilder Builder;
        [NonSerialized]
        private MethodInfo Info;

        public MethodStructure(string name, MethodAttributes attr, IReadOnlyList<GenericParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, MethodInfo info = null)
            :base(attr, arg)
        {
            Name = name;
            Generics = gnr;
            ReturnType = ret;
            AppendChild(Generics);
            Info = info;
        }

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateMethod(Name, Attributes);
            Info = Builder;
            if (Generics.Count > 0)
            {
                var gb = Builder.DefineGenericParameters(Generics.ToNames());
                Generics.RegisterBuilders(gb);
            }
            if (ReturnType != null)
            {
                Builder.SetReturnType(ReturnType.GainType()); //todo ジェネリクスに対応したTypeを生成する。
            }
            Builder.SetParameters(Arguments.ToTypes());
            Arguments.RegisterBuilders(Builder);
            SpreadGenerator();
        }

        internal override void PostBuild()
        {
            Generator.GenerateControl(OpCodes.Ret);
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            cg.GenerateCall(this);
        }

        protected override ILGenerator GainILGenerator()
        {
            return Builder.GetILGenerator();
        }

        internal MethodInfo GainMethod()
        {
            return Info;
        }
    }
}
