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
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            Name = To == null ? string.Empty : To.Value;
            base.SpreadElement(parent, scope);
        }

        internal OverLoad RefarenceResolution()
        {
            return CurrentScope.NameResolution(From.Value);
        }
    }
}
