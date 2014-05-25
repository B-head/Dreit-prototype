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
                    case 0: return SpecialTypeAccess;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
