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
        public string Name { get; private set; }
        public TypeAttributes Attributes { get; private set; }
        public IReadOnlyList<GenericParameterStructure> Generics { get; private set; }
        public TypeStructure BaseType { get; private set; }
        public IReadOnlyList<TypeStructure> Implements { get; private set; }
        [NonSerialized]
        private TypeBuilder Builder;
        [NonSerialized]
        private Type Info;

        public TypeStructure(string name, TypeAttributes attr, IReadOnlyList<GenericParameterStructure> gnr, TypeStructure bt, IReadOnlyList<TypeStructure> imp, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Generics = gnr;
            BaseType = bt;
            Implements = imp;
            Info = info;
            AppendChild(Generics);
        }

        internal Type GainType()
        {
            RelayBuildCode();
            return Info;
        }

        protected override void BuildCode()
        {
            if(Info != null)
            {
                return;
            }
            var cont = (ContainerStructure)Parent;
            Builder = cont.CreateType(Name, Attributes);
            Info = Builder;
            var gb = Builder.DefineGenericParameters(Generics.ToNames());
            Generics.RegisterBuilders(gb);
            Builder.SetParent(BaseType.GainType()); //todo ジェネリクスに対応したTypeを生成する。
            Builder.AddImplements(Implements);
        }

        internal override TypeBuilder CreateType(string name, TypeAttributes attr)
        {
            return Builder.DefineNestedType(name, attr);
        }

        internal override MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            return Builder.DefineMethod(name, attr);
        }

        internal override ConstructorBuilder CreateConstructor(MethodAttributes attr, Type[] pt)
        {
            return Builder.DefineConstructor(attr, CallingConventions.Standard, pt);
        }

        internal override FieldBuilder CreateField(string name, Type dt, FieldAttributes attr)
        {
            return Builder.DefineField(name, dt, attr);
        }
    }
}
