using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    class WithExpression : Scope
    {
        public Element Access { get; private set; }
        public ProgramContext Block { get; private set; }

        public WithExpression(TextPosition cp, Element access, ProgramContext block)
        {
            Access = access;
            Block = block;
            AppendChild(Block);
            AppendChild(Access);
        }
    }
}
