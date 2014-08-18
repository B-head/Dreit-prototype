using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using System;
using System.Collections.Generic;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Expression(SlimChainParser cp)
        {
            return SwapExpression(cp);
        }

        private static Element SwapExpression(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new SwapExpression(tp, op, l, r), LeftPipeline, TokenType.Swap);
        }

        private static Element LeftPipeline(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new LeftPipeline(tp, op, l, r), RightPipeline, TokenType.LeftPipeline);
        }

        private static Element RightPipeline(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new RightPipeline(tp, op, l, r), TupleExpression, TokenType.RightPipeline);
        }

        private static Element LeftCompose(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new Compose(tp, op, l, r), RightCompose, TokenType.LeftCompose);
        }

        private static Element RightCompose(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new Compose(tp, op, l, r), Logical, TokenType.RightCompose);
        }

        private static Element Logical(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new Logical(tp, op, l, r), Compare, TokenType.And, TokenType.Or);
        }

        private static Element Compare(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new Compare(tp, op, l, r), Addtive, TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incomparable);
        }

        private static Element Addtive(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new Calculate(tp, op, l, r), Multiplicative, TokenType.Combine, TokenType.Add, TokenType.Subtract);
        }

        private static Element Multiplicative(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new Calculate(tp, op, l, r), Prefix, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private static Element Prefix(SlimChainParser cp)
        {
            var op = TokenType.Unknoun;
            Element child = null;
            var ret = cp.Begin
                   .Type(t => op = t.TokenType, TokenType.Plus, TokenType.Minus, TokenType.Not).Lt()
                   .Transfer(e => child = e, Prefix)
                   .End(tp => new Prefix(tp, op, child));
            return ret ?? Postfix(cp);
        }

        private static Element Postfix(SlimChainParser cp)
        {
            var current = Primary(cp);
            return current == null ? null : Postfix(current, cp);
        }

        private static Element Postfix(Element current, SlimChainParser cp)
        {
            var op = TokenType.Unknoun;
            var ret = cp.Begin
                .Type(t => op = t.TokenType, TokenType.Refer, TokenType.Typeof, TokenType.Reject).Lt()
                .End(tp => new Postfix(tp, op, current));
            return ret == null ? MemberAccess(current, cp) : Postfix(ret, cp);
        }

        private static Element MemberAccess(Element current, SlimChainParser cp)
        {
            var member = string.Empty;
            var ret = cp.Begin
                .Type(TokenType.Access).Lt()
                .Take(t => member = t.Text).Lt()
                .End(tp => new MemberAccess(tp, current, member));
            return ret == null ? WithExpression(current, cp) : Postfix(ret, cp);
        }

        private static Element WithExpression(Element current, SlimChainParser cp)
        {
            ProgramContext block = null;
            var ret = cp.Begin
                .Type(TokenType.Access).Lt()
                .Transfer(e => block = e, BlockContext)
                .End(tp => new WithExpression(tp, current, block));
            return ret == null ? CallExpression(current, cp) : Postfix(ret, cp);
        }

        private static Element CallExpression(Element current, SlimChainParser cp)
        {
            TupleLiteral args = null;
            var ret = cp.Begin
                .If(icp => icp.Transfer(e => args = e, NakedArgument).Lt())
                .ElseIf(icp => icp.Type(TokenType.LeftParenthesis).Lt())
                .Than(icp => 
                {
                    icp.Transfer(e => args = e, TupleLiteral)
                    .Type(TokenType.RightParenthesis).Lt();
                })
                .Else(icp => 
                {
                    icp.Type(TokenType.LeftBracket).Lt()
                    .Transfer(e => args = e, TupleLiteral)
                    .Type(TokenType.RightBracket).Lt();
                })
                .End(tp => new CallExpression(tp, current, args));
            return ret == null ? TemplateInstance(current, cp) : Postfix(ret, cp);
        }

        private static TupleLiteral NakedArgument(SlimChainParser cp)
        {
            Element literal = null;
            return cp.Begin
                .Any(
                    icp => icp.Transfer(e => literal = e, StringLiteral),
                    icp => icp.Transfer(e => literal = e, HereDocument),
                    icp => icp.Transfer(e => literal = e, RangeLiteral),
                    icp => icp.Transfer(e => literal = e, LambdaLiteral)
                )
                .End(tp => new TupleLiteral(tp, literal));
        }

        private static Element TemplateInstance(Element current, SlimChainParser cp)
        {
            TupleLiteral args = null;
            var ret = cp.Begin
                .Type(TokenType.Template)
                .Not.Transfer(null, RangeLiteral)
                .If(icp => icp.Type(TokenType.LeftParenthesis).Lt())
                .Than(icp =>
                {
                    icp.Transfer(e => args = e, TupleLiteral)
                    .Type(TokenType.RightParenthesis).Lt();
                })
                .ElseIf(icp => icp.Type(TokenType.LeftBracket).Lt())
                .Than(icp =>
                {
                    icp.Transfer(e => args = e, TupleLiteral)
                    .Type(TokenType.RightBracket).Lt();
                })
                .Else(icp => icp.Transfer(e => args = new TupleLiteral(e.Position, e), Identifier))
                .End(tp => new TemplateInstanceExpression(tp, current, args));
            return ret == null ? current : Postfix(ret, cp);
        }
    }
}
