using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class DeclateRoutine : Scope
    {
        public List<DeclateAttribute> AttribuleList { get; set; }
        public Identifier ResultType { get; set; }
        public List<Element> Block { get; set; }
    }

    class DeclateAttribute : Scope
    {
        public Identifier Ident { get; set; }
        public Identifier ExplicitType { get; set; }
    }
}
