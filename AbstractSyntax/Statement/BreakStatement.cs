using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class BreakStatement : Element
    {
        public BreakStatement(TextPosition tp)
            :base(tp)
        {

        }

        public LoopStatement CurrentLoop()
        {
            return GetParent<LoopStatement>();
        }
    }
}
