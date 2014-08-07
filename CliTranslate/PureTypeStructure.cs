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
    public class PureTypeStructure : TypeStructure
    {
        public IReadOnlyList<GenericParameterStructure> Generics { get; private set; }
        public TypeStructure BaseType { get; private set; }
        public IReadOnlyList<TypeStructure> Implements { get; private set; }
        [NonSerialized]
        protected TypeBuilder Builder;

        public void Initialize(string name, TypeAttributes attr, IReadOnlyList<GenericParameterStructure> gnr, TypeStructure bt, IReadOnlyList<TypeStructure> imp, BlockStructure block = null, Type info = null)
        {
            Generics = gnr;
            BaseType = bt;
            Implements = imp;
            AppendChild(Generics);
            base.Initialize(name, attr, block, info);
        }

        protected override void PreBuild()
        {
            if(Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateType(Name, Attributes);
            Info = Builder;
            if (Generics.Count > 0)
            {
                var gb = Builder.DefineGenericParameters(Generics.ToNames());
                Generics.RegisterBuilders(gb);
            }
            if (BaseType != null)
            {
                Builder.SetParent(BaseType.GainType()); //todo ジェネリクスに対応したTypeを生成する。
            }
            Builder.AddImplements(Implements);
        }

        internal override void PostBuild()
        {
            if(Builder == null)
            {
                return;
            }
            Builder.CreateType();
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
