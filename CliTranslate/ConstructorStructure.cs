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
        }

        internal override void BuildCode()
        {
            var pt = CurrentContainer as PureTypeStructure;
            foreach(var f in pt.GetFields())
            {
                if(f.IsStatic)
                {
                    continue;
                }
                f.BuildInitValue(Generator);
            }
            if(SuperConstructor != null)
            {
                Generator.GenerateCode(OpCodes.Ldarg_0);
                Generator.GenerateCall(SuperConstructor);
            }
            ChildBuildCode(this);
        }

        internal override void PostBuild()
        {
            if(Generator == null)
            {
                return;
            }
            if (Block == null || !(Block.Last() is ReturnStructure))
            {
                Generator.GenerateCode(OpCodes.Ret);
            }
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
