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
            return LeftAssociative<Logical, Element>(cp, Condition, TokenType.AndElse, TokenType.OrElse);
        }
        
        private static Element Condition(ChainParser cp)
        {
            return LeftAssociative<Condition, Element>(cp, Addtive, TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incompare);
        }

        private static Element Addtive(ChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, Multiplicative, TokenType.Add, TokenType.Subtract);
        }

        private static Element Multiplicative(ChainParser cp)
        {
            return LeftAssociative<Calculate, Element>(cp, CallRoutine, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private static Element CallRoutine(ChainParser cp)
        {
            var access = MemberAccess(cp);
            var ret = cp.Begin<CallRoutine>()
                .Self(s => s.Access = access)
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer((s, e) => s.Arguments = e, c => ParseTuple(c, Addtive))
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret ?? access;
        }

        private static Element MemberAccess(ChainParser cp)
        {
            return LeftAssociative<MemberAccess, Element>(cp, Primary, TokenType.Access);
        }
    }
}
