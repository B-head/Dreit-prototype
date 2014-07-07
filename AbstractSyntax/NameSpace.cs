using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class NameSpace : Scope
    {
        public NameSpace()
        {

        }

        public NameSpace(TextPosition tp)
            : base(tp)
        {

        }

        public NameSpace(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }
    }
}
