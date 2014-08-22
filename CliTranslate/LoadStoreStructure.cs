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
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoadStoreStructure : BuilderStructure
    {
        public bool IsStore { get; private set; }
        public TypeStructure BaseInstance { get; private set; }

        public LoadStoreStructure(bool isStore)
        {
            IsStore = isStore;
        }

        internal void BuildCall(CilStructure variant, CodeGenerator cg)
        {
            var b = variant as BuilderStructure;
            if(b != null && BaseInstance != null)
            {
                variant = b.RenewInstance(BaseInstance);
            }
            var f = variant as FieldStructure;
            if(f != null)
            {
                if(f.IsEnumField)
                {
                    cg.GeneratePrimitive((dynamic)f.DefaultValue);
                }
                else if (IsStore)
                {
                    if(f.IsStatic)
                    {
                        cg.GenerateStore(f);
                        cg.GenerateLoad(f);
                    }
                    else
                    {
                        cg.GenerateCode(OpCodes.Dup);
                        var temp = new LocalStructure(f.DataType, cg);
                        cg.GenerateStore(temp);
                        cg.GenerateStore(f);
                        cg.GenerateLoad(temp);
                    }
                }
                else
                {
                    cg.GenerateLoad(f);
                }
                return;
            }
            var l = variant as LocalStructure;
            if (l != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(l);
                }
                cg.GenerateLoad(l);
                return;
            }
            var p = variant as ParameterStructure;
            if(p != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(p);
                }
                cg.GenerateLoad(p);
                return;
            }
            var lo = variant as LoopLocalStructure;
            if (lo != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(lo.Local);
                }
                cg.GenerateLoad(lo.Local);
                return;
            }
            var v = variant as ValueStructure;
            if(v != null)
            {
                cg.GeneratePrimitive(v.Value);
                return;
            }
            throw new InvalidOperationException();
        }

        internal override BuilderStructure RenewInstance(TypeStructure type)
        {
            var ret = new LoadStoreStructure(IsStore);
            ret.BaseInstance = type;
            return this;
        }
    }
}
