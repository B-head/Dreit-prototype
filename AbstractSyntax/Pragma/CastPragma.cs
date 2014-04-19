using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax.Pragma
{
    public class CastPragma : DeclateRoutine
    {
        public CastPragma()
        {
            Argument = new TupleList();
            Block = new DirectiveList();
        }

        public override bool IsPragma
        {
            get { return true; }
        }
    }
}
