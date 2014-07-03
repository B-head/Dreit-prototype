using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class NameSpace : Scope
    {
        public DirectiveList ExpList { get; set; }

        public NameSpace()
        {
            ExpList = new DirectiveList();
        }

        public void Append(Element value)
        {
            ExpList.Append(value);
        }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExpList;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }
    }
}
