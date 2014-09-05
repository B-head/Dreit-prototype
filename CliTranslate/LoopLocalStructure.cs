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
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoopLocalStructure : ExpressionStructure
    {
        public string Name { get; private set; }
        public TypeStructure DataType { get; private set; }
        public CilStructure DefaultValue { get; private set; }
        public LocalStructure Local { get; private set; }

        public LoopLocalStructure(string name, TypeStructure dt, CilStructure def)
            :base(dt)
        {
            Name = name;
            DataType = dt;
            DefaultValue = def;
            Local = new LocalStructure(name, dt);
            AppendChild(DefaultValue);
            AppendChild(Local);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            if(DefaultValue == null)
            {
                cg.GenerateLoad(Local);
            }
            else
            {
                DefaultValue.BuildCode();
                cg.GenerateStore(Local);
                cg.GenerateLoad(Local);
            }
        }
    }
}
