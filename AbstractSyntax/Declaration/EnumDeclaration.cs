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
        public Element ExplicitBaseType { get; private set; }

        public EnumDeclaration(TextPosition tp, string name, Element expli, ProgramContext block)
            :base(tp, name, block)
        {
            ExplicitBaseType = expli;
            AppendChild(ExplicitBaseType);
        }

        public override bool IsConstant
        {
            get { return true; }
        }
    }
}
