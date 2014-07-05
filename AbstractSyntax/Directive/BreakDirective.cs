using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class BreakDirective : Element
    {
        public BreakDirective(TextPosition tp)
            :base(tp)
        {

        }
    }
}
