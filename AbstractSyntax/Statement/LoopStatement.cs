using AbstractSyntax.Daclate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class LoopStatement : Scope
    {
        public Element Condition { get; set; }
        public Element On { get; set; }
        public Element By { get; set; }
        public DirectiveList Block { get; set; }

        public bool IsDefinedCondition
        {
            get { return Condition != null; }
        }

        public bool IsDefinedOn
        {
            get { return On != null; }
        }

        public bool IsDefinedBy
        {
            get { return By != null; }
        }

        public override int Count
        {
            get { return 4; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Condition;
                    case 1: return On;
                    case 2: return By;
                    case 3: return Block;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
