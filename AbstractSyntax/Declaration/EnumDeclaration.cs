using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class EnumDeclaration : EnumSymbol
    {
        public EnumDeclaration(TextPosition tp, string name, DirectiveList block)
            :base(tp, name, block)
        {

        }
    }
}
