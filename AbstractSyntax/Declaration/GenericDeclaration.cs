using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class GenericDeclaration : GenericSymbol
    {
        public Element SpecialTypeAccess { get; set; }

        public GenericDeclaration(TextPosition tp, string name, Element special)
            :base(tp, name)
        {
            SpecialTypeAccess = special;
            AppendChild(SpecialTypeAccess);
        }
    }
}
