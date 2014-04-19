using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Expression;

namespace AbstractSyntax
{
    public class AliasDirective : Element
    {
        public IdentifierAccess From { get; set; }
        public IdentifierAccess To { get; set; }

        public override bool IsVoidValue
        {
            get { return true; }
        }
    }
}
