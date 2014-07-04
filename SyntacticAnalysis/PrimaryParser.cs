using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Statement;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Primary(ChainParser cp)
        {
            return CoalesceParser(cp, primary);
        }

        private static ExpressionGroup ExpressionGroup(ChainParser cp)
        {
            return cp.Begin<ExpressionGroup>()
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer((s, e) => s.Child = e, Expression)
                .Type(TokenType.RightParenthesis).Lt()
                .End();
        }

        private static NumberLiteral NumberLiteral(ChainParser cp)
        {
            return cp.Begin<NumberLiteral>()
                .Type((s, t) => s.Integral = t.Text, TokenType.DigitStartString).Lt()
                .If().Type(TokenType.Access).Lt()
                .Than().Type((s, t) => s.Fraction = t.Text, TokenType.DigitStartString).Lt() //todo 数字で無かった場合の対応が必要。
                .EndIf().End();
        }

        private static StringLiteral StringLiteral(ChainParser cp)
        {
            return cp.Begin<StringLiteral>()
                .Type(TokenType.QuoteSeparator)
                .Loop().Not().Type(TokenType.QuoteSeparator).Do()
                .If().Type(TokenType.LeftBrace)
                .Than().Transfer((s, e) => s.Append(e), Expression).Type(TokenType.RightBrace)
                .Else().Transfer((s, e) => s.Append(e), PlainText)
                .EndIf().EndLoop().End();
        }
        private static PlainText PlainText(ChainParser cp)
        {
            return cp.Begin<PlainText>()
                .Type((s, t) => s.Value = t.Text, TokenType.PlainText)
                .End();
        }

        private static IdentifierAccess IdentifierAccess(ChainParser cp)
        {
            return cp.Begin<IdentifierAccess>()
                .Opt().Type((s, t) => s.IsPragmaAccess = true, TokenType.Pragma).Lt()
                .Type((s, t) => s.Value = t.Text, TokenType.LetterStartString).Lt()
                .End();
        }

        private static UnStatement UnStatement(ChainParser cp)
        {
            return cp.Begin<UnStatement>()
                .Text("un").Lt().Transfer((s, e) => s.Exp = e, Directive)
                .End();
        }

        private static IfStatement IfStatement(ChainParser cp)
        {
            return cp.Begin<IfStatement>()
                .Text("if").Lt().Transfer((s, e) => s.Condition = e, Directive)
                .Transfer((s, e) => s.Than = e, IfInlineDirectiveList, Block)
                .If().Text("else").Lt().Than().Transfer((s, e) => s.Else = e, PureInlineDirectiveList, Block).EndIf()
                .End();
        }

        private static LoopStatement LoopStatement(ChainParser cp)
        {
            return cp.Begin<LoopStatement>()
                .Text("loop").Lt().Opt().Transfer((s, e) => s.Condition = e, Directive)
                .If().Text("on").Lt().Than().Transfer((s, e) => s.On = e, Directive).EndIf()
                .If().Text("by").Lt().Than().Transfer((s, e) => s.By = e, Directive).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }
    }
}
