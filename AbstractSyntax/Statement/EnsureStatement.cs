using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class EnsureStatement : Scope
    {
        public Element Use { get; set; }
        public ProgramContext Block { get; set; }

        public EnsureStatement(TextPosition tp, Element use, ProgramContext block)
            :base(tp)
        {
            Use = use;
            Block = block;
            AppendChild(Use);
            AppendChild(Block);
        }
    }
}
