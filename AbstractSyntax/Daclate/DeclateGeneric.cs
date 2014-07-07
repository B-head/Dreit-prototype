using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateGeneric : GenericSymbol
    {
        public Element SpecialTypeAccess { get; set; }

        public DeclateGeneric(TextPosition tp, string name, Element special)
            :base(tp, name)
        {
            SpecialTypeAccess = special;
            AppendChild(SpecialTypeAccess);
        }
    }
}
