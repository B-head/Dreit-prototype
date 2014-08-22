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
    public class GlobalContextStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public BlockStructure Block { get; private set; }
        public PureTypeStructure GlobalField { get; private set; }
        public MethodStructure GlobalContext { get; private set; }

        public GlobalContextStructure(string name, BlockStructure block)
        {
            Name = name;
            Block = block;
            var tattr = TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.SpecialName;
            var gnr = new List<GenericParameterStructure>();
            var imp = new List<TypeStructure>();
            GlobalField = new PureTypeStructure();
            GlobalField.Initialize(Name + ".@@Global", tattr, gnr, null, imp);
            AppendChild(GlobalField);
            var mattr = MethodAttributes.PrivateScope | MethodAttributes.SpecialName | MethodAttributes.Static;
            var arg = new List<ParameterStructure>();
            var gnr2 = new List<GenericParameterStructure>();
            GlobalContext = new MethodStructure();
            GlobalContext.Initialize("@@global", false, mattr, gnr2, arg, null);
            GlobalField.AppendChild(GlobalContext);
            AppendChild(Block);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            GlobalContext.BuildCall(cg);
            ChildBuildCode(this);
        }

        internal override CodeGenerator GainGenerator()
        {
            return GlobalContext.GainGenerator();
        }

        internal override TypeBuilder CreateType(string name, System.Reflection.TypeAttributes attr)
        {
            var cont = CurrentContainer;
            return cont.CreateType(name, attr);
        }

        internal override MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            return GlobalField.CreateMethod(name, attr | MethodAttributes.Static);
        }

        internal override FieldBuilder CreateField(string name, Type dt, FieldAttributes attr)
        {
            return GlobalField.CreateField(name, dt, attr | FieldAttributes.Static);
        }
    }
}
