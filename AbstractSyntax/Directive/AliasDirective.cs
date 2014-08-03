using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class AliasDirective : Scope
    {
        public Identifier From { get; private set; }
        public Identifier To { get; private set; }

        public AliasDirective(TextPosition tp, Identifier from, Identifier to)
            :base(tp)
        {
            From = from;
            To = to;
            Name = To == null ? string.Empty : To.Value;
        }

        public override OverLoadReference OverLoad
        {
            get { return CurrentScope.NameResolution(From.Value); }
        }
    }
}
