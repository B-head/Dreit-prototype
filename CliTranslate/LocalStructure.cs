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
    public class LocalStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public TypeStructure DataType { get; private set; }
        [NonSerialized]
        private LocalBuilder Builder;

        public LocalStructure(TypeStructure dt)
        {
            DataType = dt;
        }

        public LocalStructure(string name, TypeStructure dt)
        {
            Name = name;
            DataType = dt;
        }

        public LocalStructure(TypeStructure dt, CodeGenerator cg)
        {
            DataType = dt;
            Builder = cg.CreateLocal(DataType);
        }

        protected override void PreBuild()
        {
            var cg = CurrentContainer.GainGenerator();
            Builder = cg.CreateLocal(DataType);
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Builder.SetLocalSymInfo(Name);
            }
        }

        internal LocalBuilder GainLocal()
        {
            return Builder;
        }
    }
}
