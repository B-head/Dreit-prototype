using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static Element Primary(TokenCollection c, ref int i)
        {
            return CoalesceParser(c, ref i, Group, Number, DeclateClass, DeclateRoutine, DeclateOperator, DeclareVariant, IdentifierAccess);
        }

        private static ExpressionGrouping Group(TokenCollection c, ref int i)
        {
            int temp = i;
            if (!c.CheckToken(temp, TokenType.LeftParenthesis))
            {
                return null;
            }
            var p = c.GetTextPosition(temp);
            c.MoveNextToken(ref temp);
            Element exp = DirectiveList(c, ref temp);
            if (!c.CheckToken(temp, TokenType.RightParenthesis))
            {
                return null;
            }
            p = p.AlterLength(c.GetTextPosition(i));
            c.MoveNextToken(ref temp);
            i = temp;
            return new ExpressionGrouping { Child = exp, Operator = TokenType.Special, Position = p };
        }

        private static NumberLiteral Number(TokenCollection c, ref int i)
        {
            int temp = i;
            string value = string.Empty;
            if (!c.CheckToken(temp, TokenType.DigitStartString))
            {
                return null;
            }
            Token a = c.Read(temp);
            c.MoveNextToken(ref temp);
            i = temp;
            if (c.CheckToken(temp, TokenType.Access))
            {
                c.MoveNextToken(ref temp);
                if (c.CheckToken(temp, TokenType.DigitStartString))
                {
                    Token b = c.Read(temp);
                    c.MoveNextToken(ref temp);
                    i = temp;
                    return new NumberLiteral { Integral = a.Text, Fraction = b.Text, Position = a.Position.AlterLength(b.Position) };
                }
            }
            return new NumberLiteral { Integral = a.Text, Position = a.Position };
        }

        private static IdentifierAccess IdentifierAccess(TokenCollection c, ref int i)
        {
            bool pragma = false;
            var p = c.GetTextPosition(i);
            if (c.CheckToken(i, TokenType.Pragma))
            {
                pragma = true;
                c.MoveNextToken(ref i);
            }
            if (!c.CheckToken(i, TokenType.LetterStartString))
            {
                return null;
            }
            Token t = c.Read(i);
            c.MoveNextToken(ref i);
            return new IdentifierAccess { Value = t.Text, IsPragmaAccess = pragma, Position = p.AlterLength(t.Position) };
        }
    }
}
