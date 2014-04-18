using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using AbstractSyntax;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private Element Primary(ref int c)
        {
            return CoalesceParser(ref c, Group, Number, DeclateClass, DeclateRoutine, DeclateOperator, DeclareVariant, IdentifierAccess);
        }

        private ExpressionGrouping Group(ref int c)
        {
            int temp = c;
            if (!CheckToken(temp, TokenType.LeftParenthesis))
            {
                return null;
            }
            var p = GetTextPosition(temp);
            MoveNextToken(ref temp);
            Element exp = DirectiveList(ref temp);
            if (!CheckToken(temp, TokenType.RightParenthesis))
            {
                return null;
            }
            p = SetTextLength(p, GetTextPosition(c));
            MoveNextToken(ref temp);
            c = temp;
            return new ExpressionGrouping { Child = exp, Operator = TokenType.Special, Position = p };
        }

        private NumberLiteral Number(ref int c)
        {
            int temp = c;
            string value = string.Empty;
            if (!CheckToken(temp, TokenType.DigitStartString))
            {
                return null;
            }
            Token i = Read(temp);
            MoveNextToken(ref temp);
            c = temp;
            if (CheckToken(temp, TokenType.Access))
            {
                MoveNextToken(ref temp);
                if (CheckToken(temp, TokenType.DigitStartString))
                {
                    Token f = Read(temp);
                    MoveNextToken(ref temp);
                    c = temp;
                    return new NumberLiteral { Integral = i.Text, Fraction = f.Text, Position = SetTextLength(i.Position, f.Position) };
                }
            }
            return new NumberLiteral { Integral = i.Text, Position = i.Position };
        }

        private IdentifierAccess IdentifierAccess(ref int c)
        {
            bool pragma = false;
            var p = GetTextPosition(c);
            if (CheckToken(c, TokenType.Pragma))
            {
                pragma = true;
                MoveNextToken(ref c);
            }
            if (!CheckToken(c, TokenType.LetterStartString))
            {
                return null;
            }
            Token t = Read(c);
            MoveNextToken(ref c);
            return new IdentifierAccess { Value = t.Text, IsPragmaAccess = pragma, Position = SetTextLength(p, t.Position) };
        }
    }
}
