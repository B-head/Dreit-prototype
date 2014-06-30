using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element LeftAssign(ChainParser cp)
        {
            return RightAssociative<LeftAssign, Element>(cp, RightAssign, TokenType.LeftAssign);
        }

        private static Element RightAssign(ChainParser cp)
        {
            return LeftAssociative<RightAssign, Element>(cp, TupleList, TokenType.RightAssign);
        }

        private static Element TupleList(ChainParser cp)
        {
            var tuple = ParseTuple(cp, Logical);
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

        private static Element Logical(ChainParser cp)
        {
            return RightAssociative<Logical, Element>(cp, Condition, TokenType.And, TokenType.Or);
        }
        
        private static Element Condition(ChainParser cp)
        {
            return RightAssociative<Condition, Element>(cp, Addtive, TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incompare);
        }

        private static Element Addtive(ChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, Multiplicative, TokenType.Add, TokenType.Subtract);
        }

        private static Element Multiplicative(ChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, Prefix, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private static Element Prefix(ChainParser cp)
        {
            var ret = cp.Begin<Prefix>()
                   .Type((s, t) => s.Operator = t.Type, TokenType.Add, TokenType.Subtract, TokenType.Not).Lt()
                   .Transfer((s, e) => s.Child = e, Prefix)
                   .End();
            return ret ?? Postfix(cp);
        }

        private static Element Postfix(ChainParser cp)
        {
            var current = Primary(cp);
            return current == null ? null : Postfix(current, cp);
        }

        private static Element Postfix(Element current, ChainParser cp)
        {
            var ret = cp.Begin<Postfix>()
                .Self(s => s.Child = current)
                .Type((s, t) => s.Operator = t.Type, TokenType.Refer, TokenType.Typeof).Lt()
                .End();
            return ret == null ? MemberAccess(current, cp) : Postfix(ret, cp);
        }

        private static Element MemberAccess(Element current, ChainParser cp)
        {
            var ret = cp.Begin<MemberAccess>()
                .Self(s => s.Access = current)
                .Type(TokenType.Access).Lt()
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .End();
            return ret == null ? ParenthesisCallRoutine(current, cp) : Postfix(ret, cp);
        }

        private static Element ParenthesisCallRoutine(Element current, ChainParser cp)
        {
            var ret = cp.Begin<CallRoutine>()
                .Self(s => s.CallAccess = current)
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer((s, e) => s.CallArguments = e, c => ParseTuple(c, Logical))
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret == null ? BracketCallRoutine(current, cp) : Postfix(ret, cp);
        }

        private static Element BracketCallRoutine(Element current, ChainParser cp)
        {
            var ret = cp.Begin<CallRoutine>()
                .Self(s => s.CallAccess = current)
                .Type(TokenType.LeftBracket).Lt()
                .Transfer((s, e) => s.CallArguments = e, c => ParseTuple(c, Logical))
                .Type(TokenType.RightBracket).Lt()
                .End();
            return ret == null ? current : Postfix(ret, cp);
        }
    }
}
