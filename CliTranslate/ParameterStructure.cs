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
    public class ParameterStructure : BuilderStructure
    {
        public string Name { get; private set; }
        public bool IsInstance { get; private set; }
        public bool IsThis { get; private set; }
        public ParameterAttributes Attributes { get; private set; }
        public TypeStructure ParamType { get; private set; }
        public CilStructure DefaultValue { get; private set; }
        [NonSerialized]
        private ParameterBuilder Builder;

        public ParameterStructure(string name, ParameterAttributes attr, TypeStructure pt, CilStructure def)
        {
            Name = name;
            Attributes = attr;
            ParamType = pt;
            DefaultValue = def;
            AppendChild(DefaultValue);
        }

        public ParameterStructure(TypeStructure pt)
        {
            Name = "this";
            IsThis = true;
            Attributes = ParameterAttributes.None;
            ParamType = pt;
            DefaultValue = null;
            AppendChild(DefaultValue);
        }

        internal void RegisterBuilder(ParameterBuilder builder, bool isInstance)
        {
            if(Builder != null)
            {
                throw new InvalidOperationException();
            }
            Builder = builder;
            IsInstance = isInstance;
        }

        internal int GainPosition()
        {
            if (IsThis)
            {
                return 0;
            }
            else
            {
                return IsInstance ? Builder.Position : Builder.Position - 1;
            }
        }
    }
}
