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
    public abstract class TypeStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public TypeAttributes Attributes { get; private set; }
        public BlockStructure Block { get; private set; }
        [NonSerialized]
        protected Type Info;

        protected TypeStructure()
        {
        }

        internal override bool IsDataTypeContext
        {
            get { return true; }
        }

        public void Initialize(string name, TypeAttributes attr, BlockStructure block = null, Type info = null)
        {
            Name = name;
            Attributes = attr;
            Block = block;
            AppendChild(Block);
            Info = info;
        }

        internal virtual Type GainType()
        {
            RelayPreBuild();
            return Info;
        }

        internal bool IsReferType
        {
            get { return Info.IsClass || Info.IsInterface; }
        }

        internal bool IsValueType
        {
            get { return Info.IsValueType; }
        }

        internal bool IsVoid
        {
            get { return Info == typeof(void); }
        }

        internal MethodInfo RenewMethod(MethodStructure method)
        {
            if (Info.GetType().Name == "TypeBuilderInstantiation")
            {
                return TypeBuilder.GetMethod(Info, method.GainMethod());
            }
            else
            {
                var m = method.GainMethod();
                var types = Info.RenewTypes(m.GetParameters().ToTypes());
                var ret = Info.GetMethod(m.Name, types);
                if (ret == null)
                {
                    throw new InvalidOperationException();
                }
                return ret;
            }
        }

        internal ConstructorInfo RenewConstructor(ConstructorStructure constructor)
        {
            if (Info.GetType().Name == "TypeBuilderInstantiation")
            {
                return TypeBuilder.GetConstructor(Info, constructor.GainConstructor());
            }
            else
            {
                var c = constructor.GainConstructor();
                var types = Info.RenewTypes(c.GetParameters().ToTypes());
                var ret = Info.GetConstructor(types);
                if (ret == null)
                {
                    throw new InvalidOperationException();
                }
                return ret;
            }
        }

        internal FieldInfo RenewField(FieldStructure field)
        {
            if (Info.GetType().Name == "TypeBuilderInstantiation")
            {
                return TypeBuilder.GetField(Info, field.GainField());
            }
            else
            {
                var f = field.GainField();
                var ret = Info.GetField(f.Name);
                if(ret == null)
                {
                    throw new InvalidOperationException();
                }
                return ret;
            }
        }
    }
}
