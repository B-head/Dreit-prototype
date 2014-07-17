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

        protected override void BuildCode()
        {
            if (Info != null)
            {
                return;
            }
            var cont = (ContainerStructure)Parent;
            Builder = cont.CreateMethod(Name, Attributes);
            Info = Builder;
            var gb = Builder.DefineGenericParameters(Generics.ToNames());
            Generics.RegisterBuilders(gb);
            Builder.SetReturnType(ReturnType.GainType()); //todo ジェネリクスに対応したTypeを生成する。
            Builder.SetParameters(Arguments.ToTypes());
            Arguments.RegisterBuilders(Builder);
        }
    }
}
