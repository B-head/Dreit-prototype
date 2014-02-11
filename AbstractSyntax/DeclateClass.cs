using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class DeclateClass : Scope
    {
        public Identifier Ident { get; set; }
        public Element GenericList { get; set; }
        public Element InheritList { get; set; }
        public Element Block { get; set; }

        public override int ChildCount
        {
            get { return 4; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return GenericList;
                case 2: return InheritList;
                case 3: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }
    }
}
