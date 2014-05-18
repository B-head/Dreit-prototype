using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    public class IfStatement : Scope
    {
        public Element Condition { get; set; }
        public DirectiveList Than { get; set; }
        public DirectiveList Else { get; set; }
    }
}
