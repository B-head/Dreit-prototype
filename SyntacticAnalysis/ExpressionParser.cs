using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Expression(SlimChainParser cp)
        {
            return LeftAssign(cp);
        }

        private static Element LeftAssign(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new LeftAssign(tp, op, l, r), RightAssign, TokenType.LeftAssign);
        }

        private static Element RightAssign(SlimChainParser cp)
        {
            return LeftAssociative(cp, (tp, op, l, r) => new RightAssign(tp, op, l, r), TupleList, TokenType.RightAssign);
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
            return RightAssociative(cp, (tp, op, l, r) => new Logical(tp, op, l, r), Condition, TokenType.And, TokenType.Or);
        }

        private static Element Condition(SlimChainParser cp)
        {
            return RightAssociative(cp, (tp, op, l, r) => new Condition(tp, op, l, r), Addtive, TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incompare);
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
                .Type(t => op = t.TokenType, TokenType.Refer, TokenType.Typeof).Lt()
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
                .Else(icp => icp.Transfer(e => args = new TupleList(e), IdentifierAccess))
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
                .End(tp => new CallRoutine(tp, current, args));
            return ret == null ? current : Postfix(ret, cp);
        }
    }
}
