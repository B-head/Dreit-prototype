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
            return RightAssociative<LeftAssign, Element>(cp, RightAssign, TokenType.LeftAssign);
        }

        private static Element RightAssign(SlimChainParser cp)
        {
            return LeftAssociative<RightAssign, Element>(cp, TupleList, TokenType.RightAssign);
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
            return RightAssociative<Logical, Element>(cp, Condition, TokenType.And, TokenType.Or);
        }

        private static Element Condition(SlimChainParser cp)
        {
            return RightAssociative<Condition, Element>(cp, Addtive, TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incompare);
        }

        private static Element Addtive(SlimChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, Multiplicative, TokenType.Add, TokenType.Subtract);
        }

        private static Element Multiplicative(SlimChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, Prefix, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private static Element Prefix(SlimChainParser cp)
        {
            var op = TokenType.Unknoun;
            Element child = null;
            var ret = cp.Begin
                   .Type(t => op = t.TokenType, TokenType.Add, TokenType.Subtract, TokenType.Not).Lt()
                   .Transfer(e => child = e, Prefix)
                   .End(tp => new Prefix { Child = child, Operator = op, Position = tp });
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
                .End(tp => new Postfix { Child = current, Operator = op, Position = tp });
            return ret == null ? MemberAccess(current, cp) : Postfix(ret, cp);
        }

        private static Element MemberAccess(Element current, SlimChainParser cp)
        {
            var member = string.Empty;
            var ret = cp.Begin
                .Type(TokenType.Access).Lt()
                .Take(t => member = t.Text).Lt()
                .End(tp => new MemberAccess { Access = current, Member = member, Position = tp });
            return ret == null ? ParenthesisCallRoutine(current, cp) : Postfix(ret, cp);
        }

        private static Element ParenthesisCallRoutine(Element current, SlimChainParser cp)
        {
            TupleList args = null;
            var ret = cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new Caller { Access = current, Arguments = args, Position = tp });
            return ret == null ? BracketCallRoutine(current, cp) : Postfix(ret, cp);
        }

        private static Element BracketCallRoutine(Element current, SlimChainParser cp)
        {
            TupleList args = null;
            var ret = cp.Begin
                .Type(TokenType.LeftBracket).Lt()
                .Transfer(e => args = e, c => ParseTuple(c, NonTupleExpression))
                .Type(TokenType.RightBracket).Lt()
                .End(tp => new Caller { Access = current, Arguments = args, Position = tp });
            return ret == null ? current : Postfix(ret, cp);
        }
    }
}
