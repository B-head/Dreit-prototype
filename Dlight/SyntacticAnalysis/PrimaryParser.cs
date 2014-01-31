using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Element Primary(ref int c)
        {
            return Group(ref c) ?? Number(ref c) ?? DeclareVariable(ref c) ?? Identifier(ref c);
        }

        private Element Group(ref int c)
        {
            int temp = c;
            if (!CheckToken(temp, TokenType.LeftParenthesis))
            {
                return null;
            }
            TextPosition position = Read(temp).Position;
            SkipSpaser(++temp);
            Element exp = Expression(ref temp);
            if (!CheckToken(temp, TokenType.RightParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            c = temp;
            return new ExpressionGrouping { Child = exp, Operation = TokenType.Special, Position = position };
        }

        private Element Number(ref int c)
        {
            int temp = c;
            string value = string.Empty;
            if (!CheckToken(temp, TokenType.DigitStartString))
            {
                return null;
            }
            Token i = Read(temp);
            SkipSpaser(++temp);
            c = temp;
            if (CheckToken(temp, TokenType.Access))
            {
                SkipSpaser(++temp);
                if (CheckToken(temp, TokenType.DigitStartString))
                {
                    Token f = Read(temp);
                    SkipSpaser(++temp);
                    c = temp;
                    return new NumberLiteral { Integral = i.Text, Fraction = f.Text, Position = i.Position };
                }
            }
            return new NumberLiteral { Integral = i.Text, Position = i.Position };
        }

        private Element Identifier(ref int c)
        {
            if (!CheckToken(c, TokenType.LetterStartString))
            {
                return null;
            }
            Token t = Read(c);
            SkipSpaser(++c);
            return new Identifier { Value = t.Text, Position = t.Position };
        }

        private Element SpecialIdentifier(ref int c)
        {
            if (!CheckText(c, "this"))
            {
                return null;
            }
            Token t = Read(c);
            SkipSpaser(++c);
            return new Identifier { Value = t.Text, Position = t.Position };
        }

        private Element DeclareVariable(ref int c)
        {
            if (!CheckText(c, "var"))
            {
                return null;
            }
            SkipSpaser(++c);
            Identifier name = (Identifier)Identifier(ref c);
            if (!CheckToken(c, TokenType.Peir))
            {
                return new DeclareVariant { Ident = name, Position = name.Position };
            }
            SkipSpaser(++c);
            Identifier dataType = (Identifier)Identifier(ref c);
            return new DeclareVariant { Ident = name, ExplicitType = dataType, Position = name.Position };
        }
    }
}
