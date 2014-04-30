using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class AliasDirective : Scope
    {
        public IdentifierAccess From { get; set; }
        public IdentifierAccess To { get; set; }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            Name = To == null ? string.Empty : To.Value;
            base.SpreadElement(parent, scope);
        }

        public OverLoad RefarenceResolution()
        {
            return CurrentScope.NameResolution(From.Value);
        }
    }
}
