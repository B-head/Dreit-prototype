using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class ForStatement : Scope
    {
        public Element Condition { get; private set; }
        public Element Of { get; private set; }
        public Element At { get; private set; }
        public ProgramContext Block { get; private set; }

        public ForStatement(TextPosition tp, Element cond, Element of, Element at, ProgramContext block)
            :base(tp)
        {
            Condition = cond;
            Of = of;
            At = at;
            Block = block;
            AppendChild(Condition);
            AppendChild(Of);
            AppendChild(At);
            AppendChild(Block);
        }
    }
}
