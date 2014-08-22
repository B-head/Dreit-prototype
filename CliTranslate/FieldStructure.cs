/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
