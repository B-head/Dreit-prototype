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
            DeclateClass,
            DeclateRoutine,
            DeclateOperator,
            DeclareVariant,
            IfStatement,
            LoopStatement,
            UnStatement,
            ExpressionGroup,
            StringLiteral,
            NumberLiteral,
            IdentifierAccess
        };

        private static Element Primary(SlimChainParser cp)
        {
            return CoalesceParser(cp, primary);
        }

        private static ExpressionGroup ExpressionGroup(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer(e => exp = e, Expression)
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new ExpressionGroup(tp, exp));
        }

        private static NumberLiteral NumberLiteral(SlimChainParser cp)
        {
            var integral = string.Empty;
            var fraction = string.Empty;
            return cp.Begin
                .Type(t => integral = t.Text, TokenType.DigitStartString).Lt()
                .If(icp => icp.Type(TokenType.Access).Lt())
                .Than(icp => icp.Type(t => fraction = t.Text, TokenType.DigitStartString).Lt())
                .End(tp => new NumberLiteral(tp, integral, fraction));
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

        private static IdentifierAccess IdentifierAccess(SlimChainParser cp)
        {
            var isPragma = false;
            var value = string.Empty;
            return cp.Begin
                .Opt.Type(t => isPragma = true, TokenType.Pragma).Lt()
                .Type(t => value = t.Text, TokenType.LetterStartString).Lt()
                .End(tp => new IdentifierAccess(tp, value, isPragma));
        }

        private static IdentifierAccess IdentifierAccess(SlimChainParser cp, params string[] match)
        {
            var isPragma = false;
            var value = string.Empty;
            return cp.Begin
                .Opt.Type(t => isPragma = true, TokenType.Pragma).Lt()
                .Text(t => value = t.Text, match).Lt()
                .End(tp => new IdentifierAccess(tp, value, isPragma));
        }

        private static UnStatement UnStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("un").Lt()
                .Transfer(e => exp = e, Directive)
                .End(tp => new UnStatement(tp, exp));
        }

        private static IfStatement IfStatement(SlimChainParser cp)
        {
            Element cond = null;
            DirectiveList than = null;
            DirectiveList els = null;
            return cp.Begin
                .Text("if").Lt()
                .Transfer(e => cond = e, Directive)
                .Transfer(e => than = e, icp => Block(icp, true))
                .If(icp => icp.Text("else").Lt())
                .Than(icp => icp.Transfer(e => els = e, iicp => Block(iicp, false)))
                .End(tp => new IfStatement(tp, cond, than, els));
        }

        private static LoopStatement LoopStatement(SlimChainParser cp)
        {
            Element cond = null;
            Element on = null;
            Element by = null;
            DirectiveList block = null;
            return cp.Begin
                .Text("loop").Lt()
                .Opt.Transfer(e => cond = e, Directive)
                .If(icp => icp.Text("on").Lt())
                .Than(icp => icp.Transfer(e => on = e, Directive))
                .If(icp => icp.Text("by").Lt())
                .Than(icp => icp.Transfer(e => by = e, Directive))
                .Transfer(e => block = e, icp => Block(icp, true))
                .End(tp => new LoopStatement(tp, cond, on, by, block));
        }
    }
}
