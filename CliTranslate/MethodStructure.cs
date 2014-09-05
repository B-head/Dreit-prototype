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
    public class MethodStructure : MethodBaseStructure
    {
        public string Name { get; private set; }
        public IReadOnlyList<GenericParameterStructure> Generics { get; private set; }
        public TypeStructure ReturnType { get; private set; }
        public bool IsDefaultThisReturn { get; private set; }
        [NonSerialized]
        private MethodBuilder Builder;
        [NonSerialized]
        private MethodInfo Info;

        public void Initialize(string name, bool isInstance, MethodAttributes attr, IReadOnlyList<GenericParameterStructure> gnr, IReadOnlyList<ParameterStructure> arg, TypeStructure ret, BlockStructure block = null, bool isDtr = false, MethodInfo info = null)
        {
            Name = name;
            Generics = gnr;
            ReturnType = ret;
            IsDefaultThisReturn = isDtr;
            AppendChild(Generics);
            Info = info;
            base.Initialize(isInstance, attr, arg, block);
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
            Arguments.RegisterBuilders(Builder, IsInstance);
            SpreadGenerator();
        }

        public bool IsVoidReturn
        {
            get { return ReturnType == null || ReturnType.IsVoid; }
        }

        public bool IsVirtual
        {
            get { return Attributes.HasFlag(MethodAttributes.Virtual); }
        }

        internal override void PostBuild()
        {
            if(Generator == null)
            {
                return;
            }
            if (Block == null || !(Block.Last() is ReturnStructure))
            {
                if (IsDefaultThisReturn)
                {
                    Generator.GenerateCode(OpCodes.Ldarg_0);
                }
                Generator.GenerateCode(OpCodes.Ret);
            }
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

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            var ret = new MethodStructure();
            ret.Info = type.RenewMethod(this);
            return ret;
        }
    }
}
