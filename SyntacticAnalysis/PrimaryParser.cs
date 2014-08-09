using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Statement;
using System.Collections.Generic;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static TransferParser<Element>[] primary = 
        { 
            AliasDeclaration,
            VariantDeclaration,
            RoutineDeclaration,
            OperatorDeclaration,
            ClassDeclaration,
            AttributeScope,
            IfStatement,
            LoopStatement,
            UnStatement,
            EchoStatement,
            ReturnStatement,
            BreakStatement,
            ContinueStatement,
            StringLiteral,
            NumericLiteral,
            GroupingExpression,
            Identifier
        };

        private static Element Primary(SlimChainParser cp)
        {
            return CoalesceParser(cp, primary);
        }

        private static GroupingExpression GroupingExpression(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer(e => exp = e, Expression)
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new GroupingExpression(tp, exp));
        }

        private static NumericLiteral NumericLiteral(SlimChainParser cp)
        {
            var integral = string.Empty;
            var fraction = string.Empty;
            return cp.Begin
                .Type(t => integral = t.Text, TokenType.DigitStartString).Lt()
                .If(icp => icp.Type(TokenType.Access).Lt())
                .Than(icp => icp.Type(t => fraction = t.Text, TokenType.DigitStartString).Lt())
                .End(tp => new NumericLiteral(tp, integral, fraction));
        }

        private static StringLiteral StringLiteral(SlimChainParser cp)
        {
            var texts = new List<Element>();
            return cp.Begin
                .Type(TokenType.QuoteSeparator)
                .Loop(icp => icp.Not.Type(TokenType.QuoteSeparator), icp =>
                {
                    icp
                    .If(iicp => iicp.Type(TokenType.LeftBrace))
                    .Than(iicp => iicp.Transfer(e => texts.Add(e), Expression).Type(TokenType.RightBrace))
                    .Else(iicp => iicp.Transfer(e => texts.Add(e), PlainText));
                })
                .End(tp => new StringLiteral(tp, texts));
        }

        private static PlainText PlainText(SlimChainParser cp)
        {
            var value = string.Empty;
            return cp.Begin
                .Type(t => value = t.Text, TokenType.PlainText)
                .End(tp => new PlainText(tp, value));
        }

        private static Identifier Identifier(SlimChainParser cp)
        {
            var isPragma = false;
            var value = string.Empty;
            return cp.Begin
                .Opt.Type(t => isPragma = true, TokenType.Pragma).Lt()
                .Type(t => value = t.Text, TokenType.LetterStartString).Lt()
                .End(tp => new Identifier(tp, value, isPragma));
        }

        private static Identifier Identifier(SlimChainParser cp, params string[] match)
        {
            var isPragma = false;
            var value = string.Empty;
            return cp.Begin
                .Opt.Type(t => isPragma = true, TokenType.Pragma).Lt()
                .Text(t => value = t.Text, match).Lt()
                .End(tp => new Identifier(tp, value, isPragma));
        }
    }
}
