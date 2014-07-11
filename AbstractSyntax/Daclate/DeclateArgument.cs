using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateArgument : ArgumentSymbol
    {
        public TupleList AttributeAccess { get; private set; }
        public IdentifierAccess Ident { get; private set; }
        public IdentifierAccess ExplicitType { get; private set; }

        public DeclateArgument(TextPosition tp, TupleList attr, IdentifierAccess ident, IdentifierAccess expl)
            :base(tp)
        {
            AttributeAccess = attr;
            Ident = ident;
            ExplicitType = expl;
            Name = Ident == null ? string.Empty : Ident.Value;
            AppendChild(AttributeAccess);
            AppendChild(Ident);
            AppendChild(ExplicitType);
        }
    }
}
