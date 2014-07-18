using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class ContinueDirective : Element
    {
        public ContinueDirective(TextPosition tp)
            :base(tp)
        {

        }

        public LoopStatement CurrentLoop()
        {
            return GetParent<LoopStatement>();
        }
    }
}
