using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using AbstractSyntax.Expression;

namespace AbstractSyntax.Daclate
{
    public class DeclateEnum : Scope
    {
        public IdentifierAccess Ident { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? Name : Ident.Value;
        }
    }
}
