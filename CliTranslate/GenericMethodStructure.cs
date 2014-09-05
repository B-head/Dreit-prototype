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
    public class GenericMethodStructure : BuilderStructure
    {
        public BuilderStructure BaseInstance { get; private set; }
        public IReadOnlyList<TypeStructure> GenericParameter { get; private set; }
        public TypeStructure DeclaringInstance { get; private set; }
        [NonSerialized]
        private MethodInfo Info;

        public void Initialize(BuilderStructure inst, IReadOnlyList<TypeStructure> prms, TypeStructure di, MethodInfo info = null)
        {
            BaseInstance = inst;
            GenericParameter = prms;
            DeclaringInstance = di;
            Info = info;
        }

        public BuilderStructure RenewBaseInstance
        {
            get { return BaseInstance.RenewInstance(DeclaringInstance); }
        }

        internal MethodInfo GainMethod()
        {
            if (Info != null)
            {
                return Info;
            }
            Info = GainBase();
            if(GenericParameter.Count > 0)
            {
                Info = Info.MakeGenericMethod(GenericParameter.GainTypes());
            }
            return Info;
        }

        private MethodInfo GainBase()
        {
            var m = RenewBaseInstance as MethodStructure;
            if (m != null)
            {
                return m.GainMethod();
            }
            throw new InvalidOperationException();
        }

        internal void BuildCall(CilStructure variant, CodeGenerator cg)
        {
            var ls = RenewBaseInstance as LoadStoreStructure;
            if(ls != null)
            {
                ls.BuildCall(variant, cg);
            }
            else if (RenewBaseInstance is MethodStructure)
            {
                cg.GenerateCall(this);
            }
            else
            {
                RenewBaseInstance.BuildCall(cg);
            }
        }
    }
}
