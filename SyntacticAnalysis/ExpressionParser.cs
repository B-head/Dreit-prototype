using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Expression(TokenCollection c, ref int i)
        {
            return LeftAssign(c, ref i);
        }

        private static Element LeftAssign(TokenCollection c, ref int i)
        {
            return RightAssociative<LeftAssign>(c, ref i, RightAssign, TokenType.LeftAssign);
        }

        private static Element RightAssign(TokenCollection c, ref int i)
        {
            return LeftAssociative<RightAssign>(c, ref i, TupleList, TokenType.RightAssign);
        }

        private static Element TupleList(TokenCollection c, ref int i)
        {
            var tuple = ParseTuple(c, ref i, Addtive);
            if (tuple.Count > 1)
            {
                return tuple;
            }
            if (tuple.Count > 0)
            {
                return tuple[0];
            }
            else
            {
                return null;
            }
        }

        private static Element Addtive(TokenCollection c, ref int i)
        {
            return LeftAssociative<DyadicCalculate>(c, ref i, Multiplicative, TokenType.Add, TokenType.Subtract, TokenType.Combine);
        }

        private static Element Multiplicative(TokenCollection c, ref int i)
        {
            return LeftAssociative<DyadicCalculate>(c, ref i, Exponentive, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private static Element Exponentive(TokenCollection c, ref int i)
        {
            return RightAssociative<DyadicCalculate>(c, ref i, CallRoutine, TokenType.Exponent);
        }

        private static Element CallRoutine(TokenCollection c, ref int i)
        {
            var p = c.GetTextPosition(i);
            var access = MemberAccess(c, ref i);
            int temp = i;
            if (!c.CheckToken(temp, TokenType.LeftParenthesis))
            {
                return access;
            }
            c.MoveNextToken(ref temp);
            var argument = ParseTuple(c, ref temp, Addtive); //todo デフォルト引数などに対応した専用の構文が必要。
            if (!c.CheckToken(temp, TokenType.RightParenthesis))
            {
                return access;
            }
            p = p.AlterLength(c.GetTextPosition(temp));
            c.MoveNextToken(ref temp);
            i = temp;
            return new CallRoutine { Access = access, Argument = argument, Position = p };
        }

        private static Element MemberAccess(TokenCollection c, ref int i)
        {
            return LeftAssociative<MemberAccess>(c, ref i, Primary, TokenType.Access);
        }
    }
}
