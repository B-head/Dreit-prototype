using AbstractSyntax;
using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static ExpressionList RootExpressionList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var result = cp.Begin
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator)
                .Loop(icp => icp.Readable(), icp =>
                {
                    icp.Any(
                        iicp => iicp.Transfer(e => child.Add(e), Expression),
                        iicp => iicp.AddError()
                    )
                    .Ignore(TokenType.EndExpression, TokenType.LineTerminator);
                })
                .End(tp => new ExpressionList(tp, child, false));
            if (result == null)
            {
                throw new InvalidOperationException();
            }
            return result;
        }

        private static ExpressionList BlockExpressionList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var result = cp.Begin
                .Type(TokenType.LeftBrace)
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator)
                .Loop(icp => icp.Not.Type(TokenType.RightBrace), icp =>
                {
                    icp.Any(
                        iicp => iicp.Transfer(e => child.Add(e), Expression),
                        iicp => iicp.AddError()
                    )
                    .Ignore(TokenType.EndExpression, TokenType.LineTerminator);
                })
                .End(tp => new ExpressionList(tp, child, false));
            if (result == null)
            {
                result = new ExpressionList();
            }
            return result;
        }

        private static ExpressionList InlineExpressionList(SlimChainParser cp, bool separator)
        {
            var child = new List<Element>();
            return cp.Begin
                .If(icp => icp.Is(separator))
                .Than(icp =>
                {
                    icp.Any(
                        iicp => iicp.Type(TokenType.Separator),
                        iicp => iicp.Text("then")
                    ).Lt();
                })
                .Transfer(e => child.Add(e), Expression)
                .End(tp => new ExpressionList(tp, child, true));
        }

        private static ExpressionList Block(SlimChainParser cp, bool separator)
        {
            return CoalesceParser<ExpressionList>(cp, icp => InlineExpressionList(icp, separator), BlockExpressionList);
        }

        private static Element Expression(SlimChainParser cp)
        {
            return LeftPipeline(cp);
        }

        private static Element LeftPipeline(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new LeftPipeline(tp, op, l, r), RightPipeline, TokenType.LeftPipeline);
        }

        private static Element RightPipeline(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new RightPipeline(tp, op, l, r), TupleList, TokenType.RightPipeline);
        }

        private static Element TupleList(SlimChainParser cp)
        {
            var tuple = ParseTuple(cp, NonTupleExpression);
            if (tuple.Count > 1)
            {
                return tuple;
            }
            else if (tuple.Count > 0)
            {
                return (Element)tuple[0];
            }
            else
            {
                return null;
            }
        }
        private static Element NonTupleExpression(SlimChainParser cp)
        {
            return Logical(cp);
        }

        private static Element Logical(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new Logical(tp, op, l, r), Compare, TokenType.And, TokenType.Or);
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
            return ret == null ? TemplateInstance(current, cp) : Postfix(ret, cp);
        }

        private static Element TemplateInstance(Element current, SlimChainParser cp)
        {
            TupleList args = null;
            var ret = cp.Begin
                .Type(TokenType.Template)
                .If(icp => icp.Type(TokenType.LeftParenthesis).Lt())
                .Than(icp =>
                {
                    icp.Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                    .Type(TokenType.RightParenthesis).Lt();
                })
                .ElseIf(icp => icp.Type(TokenType.LeftBracket).Lt())
                .Than(icp =>
                {
                    icp.Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                    .Type(TokenType.RightBracket).Lt();
                })
                .Else(icp => icp.Transfer(e => args = new TupleList(e), Identifier))
                .End(tp => new TemplateInstanceExpression(tp, current, args));
            return ret == null ? CallRoutine(current, cp) : Postfix(ret, cp);
        }

        private static Element CallRoutine(Element current, SlimChainParser cp)
        {
            TupleList args = null;
            var ret = cp.Begin
                .If(icp => icp.Type(TokenType.LeftParenthesis).Lt())
                .Than(icp => 
                {
                    icp.Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                    .Type(TokenType.RightParenthesis).Lt();
                })
                .Else(icp => 
                {
                    icp.Type(TokenType.LeftBracket).Lt()
                    .Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                    .Type(TokenType.RightBracket).Lt();
                })
                .End(tp => new CallExpression(tp, current, args));
            return ret == null ? current : Postfix(ret, cp);
        }
    }
}
