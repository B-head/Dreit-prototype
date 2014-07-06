using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class AliasDirective : Scope
    {
        public IdentifierAccess From { get; private set; }
        public IdentifierAccess To { get; private set; }

        public AliasDirective(TextPosition tp, IdentifierAccess from, IdentifierAccess to)
            :base(tp)
        {
            From = from;
            To = to;
            Name = To == null ? string.Empty : To.Value;
        }

        internal OverLoad RefarenceResolution()
        {
            return CurrentScope.NameResolution(From.Value);
        }
    }
}
