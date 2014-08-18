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
        public object ConstantValue { get; private set; }
        [NonSerialized]
        private FieldBuilder Builder;
        [NonSerialized]
        private FieldInfo Info;

        public FieldStructure(string name, FieldAttributes attr, TypeStructure dt, object constval, FieldInfo info = null)
        {
            Name = name;
            Attributes = attr;
            DataType = dt;
            ConstantValue = constval;
            Info = info;
        }

        public bool IsStatic
        {
            get { return Info.IsStatic; }
        }

        internal void BuildInitValue(CodeGenerator cg)
        {
            if(ConstantValue == null)
            {
                return;
            }
            cg.GenerateControl(OpCodes.Ldarg_0);
            cg.GeneratePrimitive((dynamic)ConstantValue);
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
            if (ConstantValue != null)
            {
                Builder.SetConstant(ConstantValue);
            }
        }

        internal FieldInfo GainField()
        {
            return Info;
        }

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            return new FieldStructure(Name, Attributes, DataType, ConstantValue, type.RenewField(this));;
        }
    }
}
