using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Syntax Expression(ref int c)
        {
            Syntax result = Assign(ref c);
            if (CheckToken(c, TokenType.EndExpression))
            {
                SkipSpaser(++c);
            }
            return result;
        }

        private Syntax Assign(ref int c)
        {
            return RepeatSet<Assign>(ref c, Bitwise, TokenType.LeftAssign, TokenType.RightAssign);
        }

        private Syntax Bitwise(ref int c)
        {
            return LeftJoinBinomial(ref c, Shift, TokenType.Or, TokenType.And, TokenType.Xor);
        }

        private Syntax Shift(ref int c)
        {
            return LeftJoinBinomial(ref c, Addtive, TokenType.LeftShift, TokenType.RightShift);
        }

        private Syntax Addtive(ref int c)
        {
            return LeftJoinBinomial(ref c, Multiplicative, TokenType.Add, TokenType.Subtract, TokenType.Combine);
        }

        private Syntax Multiplicative(ref int c)
        {
            return LeftJoinBinomial(ref c, Exponentive, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private Syntax Exponentive(ref int c)
        {
            return LeftJoinBinomial(ref c, Primary, TokenType.Exponent);
        }

        private Syntax Primary(ref int c)
        {
            return Group(ref c) ?? Number(ref c) ?? DeclareVariable(ref c) ?? Identifier(ref c);
        }

        private Syntax Group(ref int c)
        {
            int temp = c;
            if (!CheckToken(temp, TokenType.LeftParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            Syntax result = Expression(ref temp);
            if (!CheckToken(temp, TokenType.RightParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            c = temp;
            return result;
        }

        private Syntax Number(ref int c)
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

        private Syntax Identifier(ref int c)
        {
            if (!CheckToken(c, TokenType.LetterStartString))
            {
                return null;
            }
            Token t = Read(c);
            SkipSpaser(++c);
            return new Identifier { Value = t.Text, Position = t.Position };
        }

        private Syntax DeclareVariable(ref int c)
        {
            if (!CheckText(c, "var"))
            {
                return null;
            }
            SkipSpaser(++c);
            Identifier name = (Identifier)Identifier(ref c);
            if (!CheckToken(c, TokenType.Peir))
            {
                return new DeclareVariable { Name = name, Position = name.Position };
            }
            SkipSpaser(++c);
            Identifier dataType = (Identifier)Identifier(ref c);
            return new DeclareVariable { Name = name, DataType = dataType, Position = name.Position };
        }
    }
}
