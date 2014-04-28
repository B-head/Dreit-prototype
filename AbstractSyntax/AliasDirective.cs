using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class AliasDirective : Element
    {
        public IdentifierAccess From { get; set; }
        public IdentifierAccess To { get; set; }
    }
}
