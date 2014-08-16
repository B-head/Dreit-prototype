using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class AliasDeclaration : Scope
    {
        public Identifier From { get; private set; }
        public Identifier To { get; private set; }

        public AliasDeclaration(TextPosition tp, Identifier from, Identifier to)
            :base(tp)
        {
            From = from;
            To = to;
            Name = To == null ? string.Empty : To.Value;
        }
    }
}
