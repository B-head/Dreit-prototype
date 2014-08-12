using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class EnumDeclaration : EnumSymbol
    {
        public TupleLiteral AttributeAccess { get; private set; }
        public TupleLiteral DecGenerics { get; private set; }
        public Element ExplicitBaseType { get; private set; }

        public EnumDeclaration(TextPosition tp, string name, TupleLiteral attr, TupleLiteral generic, Element expli, ProgramContext block)
            :base(tp, name, block)
        {
            AttributeAccess = attr;
            DecGenerics = generic;
            ExplicitBaseType = expli;
            AppendChild(AttributeAccess);
            AppendChild(DecGenerics);
            AppendChild(ExplicitBaseType);
        }

        public override bool IsConstant
        {
            get { return true; }
        }
    }
}
