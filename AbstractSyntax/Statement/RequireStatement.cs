using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class RequireStatement : Scope
    {
        public ProgramContext Block { get; set; }

        public RequireStatement(TextPosition tp, ProgramContext block)
            :base(tp)
        {
            Block = block;
            AppendChild(Block);
        }
    }
}
