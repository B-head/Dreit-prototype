using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class IfStatement : Scope
    {
        public Element Condition { get; set; }
        public DirectiveList Than { get; set; }
        public DirectiveList Else { get; set; }

        public bool IsDefinedElse
        {
            get { return Else != null; }
        }

        public override int Count
        {
            get { return 3; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Condition;
                    case 1: return Than;
                    case 2: return Else;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
