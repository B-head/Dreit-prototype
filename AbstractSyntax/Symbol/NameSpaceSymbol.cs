using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class NameSpaceSymbol : Scope
    {
        public NameSpaceSymbol()
        {

        }

        public NameSpaceSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public NameSpaceSymbol(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }
    }
}
