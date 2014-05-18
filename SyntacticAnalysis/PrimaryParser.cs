using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Statement;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Primary(ChainParser cp)
        {
            return CoalesceParser(cp, primary);
        }

        private static ExpressionGroup Group(ChainParser cp)
        {
            return cp.Begin<ExpressionGroup>()
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer((s, e) => s.Child = e, Directive)
                .Type(TokenType.RightParenthesis).Lt()
                .End();
        }

        private static NumberLiteral Number(ChainParser cp)
        {
            return cp.Begin<NumberLiteral>()
                .Type((s, t) => s.Integral = t.Text, TokenType.DigitStartString).Lt()
                .If().Type(TokenType.Access).Lt()
                .Than().Type((s, t) => s.Fraction = t.Text, TokenType.DigitStartString).Lt() //todo 数字で無かった場合の対応が必要。
                .EndIf().End();
        }

        private static IdentifierAccess IdentifierAccess(ChainParser cp)
        {
            return cp.Begin<IdentifierAccess>()
                .Opt().Type((s, t) => s.IsPragmaAccess = true, TokenType.Pragma).Lt()
                .Type((s, t) => s.Value = t.Text, TokenType.LetterStartString).Lt()
                .End();
        }

        private static IfStatement IfStatement(ChainParser cp)
        {
            return cp.Begin<IfStatement>()
                .Text("if").Transfer((s, e) => s.Condition = e, Condition)
                .Type(TokenType.Separator).Or().Text("than").Transfer((s, e) => s.Than = e, Block)
                .If().Text("else").Than().Text("than").Transfer((s, e) => s.Than = e, Block).EndIf()
                .End();
        }
    }
}
