using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class CatchStatement : Scope
    {
        public Element Use { get; private set; }
        public ProgramContext Block { get; private set; }

        public CatchStatement(TextPosition tp, Element use, ProgramContext block)
            :base(tp)
        {
            Use = use;
            Block = block;
            AppendChild(Use);
            AppendChild(Block);
        }
    }
}
