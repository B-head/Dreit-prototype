using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Element LeftJoinBinomial(ref int c, ParserFunction next, params TokenType[] type)
        {
            Element left = next(ref c);
            if (left == null)
            {
                return null;
            }
            while (IsReadable(c))
            {
                TokenType match;
                if (!CheckToken(c, out match, type))
                {
                    break;
                }
                SkipSpaser(++c);
                Element right = next(ref c);
                left = new Binomial { Left = left, Right = right, Operation = match, Position = left.Position };
            }
            return left;
        }

        private Element RepeatSet<SET>(ref int c, ParserFunction next, params TokenType[] type) where SET : ExpressionSet, new()
        {
            List<Element> child = new List<Element>();
            List<TokenType> expType = new List<TokenType>();
            Element first = next(ref c);
            if (first == null)
            {
                return null;
            }
            child.Add(first);
            while (IsReadable(c))
            {
                TokenType match;
                if (!CheckToken(c, out match, type))
                {
                    break;
                }
                expType.Add(match);
                SkipSpaser(++c);
                child.Add(next(ref c));
            }
            if (child.Count <= 1)
            {
                return first;
            }
            return new SET { Child = child, ExpType = expType, Position = first.Position };
        }

        private Element Expression(ref int c)
        {
            Element result = Assign(ref c);
            if (CheckToken(c, TokenType.EndExpression))
            {
                SkipSpaser(++c);
            }
            return result;
        }

        private Element Assign(ref int c)
        {
            return RepeatSet<Assign>(ref c, Bitwise, TokenType.LeftAssign, TokenType.RightAssign);
        }

        private Element Bitwise(ref int c)
        {
            return LeftJoinBinomial(ref c, Shift, TokenType.Or, TokenType.And, TokenType.Xor);
        }

        private Element Shift(ref int c)
        {
            return LeftJoinBinomial(ref c, Addtive, TokenType.LeftShift, TokenType.RightShift);
        }

        private Element Addtive(ref int c)
        {
            return LeftJoinBinomial(ref c, Multiplicative, TokenType.Add, TokenType.Subtract, TokenType.Combine);
        }

        private Element Multiplicative(ref int c)
        {
            return LeftJoinBinomial(ref c, Exponentive, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private Element Exponentive(ref int c)
        {
            return LeftJoinBinomial(ref c, Primary, TokenType.Exponent);
        }

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
            SkipSpaser(++temp);
            Element result = Expression(ref temp);
            if (!CheckToken(temp, TokenType.RightParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            c = temp;
            return result;
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
            return new DeclareVariant { Ident = name, DataType = dataType, Position = name.Position };
        }
    }
}
