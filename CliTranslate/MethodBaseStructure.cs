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
    public abstract class MethodBaseStructure : ContainerStructure
    {
        public bool IsInstance { get; private set; }
        public MethodAttributes Attributes { get; private set; }
        public IReadOnlyList<ParameterStructure> Arguments { get; private set; }
        public BlockStructure Block { get; private set; }
        public TypeStructure Lexical { get; private set; }
        public CodeGenerator Generator { get; private set; }

        protected abstract ILGenerator GainILGenerator();

        protected MethodBaseStructure()
        {
        }

        public void Initialize(bool isInstance, MethodAttributes attr, IReadOnlyList<ParameterStructure> arg, BlockStructure block = null)
        {
            IsInstance = isInstance;
            Attributes = attr;
            Arguments = arg;
            Block = block;
            AppendChild(Arguments);
            AppendChild(Block);
        }

        protected void SpreadGenerator()
        {
            var il = GainILGenerator();
            Generator = new CodeGenerator(il);
        }

        internal override CodeGenerator GainGenerator()
        {
            return Generator;
        }
    }
}
