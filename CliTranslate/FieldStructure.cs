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
    public class FieldStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public FieldAttributes Attributes { get; private set; }
        public TypeStructure DataType { get; private set; }
        public object DefaultValue { get; private set; }
        public bool IsEnumField { get; private set; }
        [NonSerialized]
        private FieldBuilder Builder;
        [NonSerialized]
        private FieldInfo Info;

        private FieldStructure()
        {

        }

        public FieldStructure(string name, FieldAttributes attr, TypeStructure dt, object def, bool isEnumField, FieldInfo info = null)
        {
            Name = name;
            Attributes = attr;
            DataType = dt;
            DefaultValue = def;
            IsEnumField = isEnumField;
            Info = info;
        }

        public bool IsStatic
        {
            get { return Info.IsStatic; }
        }

        internal void BuildInitValue(CodeGenerator cg)
        {
            if(DefaultValue == null)
            {
                return;
            }
            if (!IsStatic)
            {
                cg.GenerateCode(OpCodes.Ldarg_0);
            }
            cg.GeneratePrimitive((dynamic)DefaultValue);
            cg.GenerateStore(this);
        }

        protected override void PreBuild()
        {
            if (Info != null)
            {
                return;
            }
            var cont = CurrentContainer;
            Builder = cont.CreateField(Name, DataType.GainType(), Attributes);
            Info = Builder;
            if (DefaultValue != null)
            {
                Builder.SetConstant(DefaultValue);
            }
        }

        internal FieldInfo GainField()
        {
            return Info;
        }

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            var ret = new FieldStructure();
            ret.Info = type.RenewField(this);
            return ret;
        }
    }
}
