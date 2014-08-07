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
