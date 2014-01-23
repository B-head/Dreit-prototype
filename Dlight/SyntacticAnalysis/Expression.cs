using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Syntax LeftJoinBinomial(ref int c, ParserFunction next, params SyntaxType[] type)
        {
            Syntax left = next(ref c);
            if(left == null)
            {
                return null;
            }
            while(IsReadable(c))
            {
                SyntaxType match;
                if(!CheckToken(c, out match, type))
                {
                    break;
                }
                SkipSpaser(++c);
                Syntax right = next(ref c);
                left = new Binomial { Left = left, Right = right, Operation = match, Position = left.Position };
            }
            return left;
        }

        private Syntax Expression(ref int c)
        {
            Syntax result = Bitwise(ref c);
            if (CheckToken(c, SyntaxType.EndExpression))
            {
                SkipSpaser(++c);
            }
            return result;
        }

        private Syntax Bitwise(ref int c)
        {
            return LeftJoinBinomial(ref c, Shift, SyntaxType.Or, SyntaxType.And, SyntaxType.Xor);
        }

        private Syntax Shift(ref int c)
        {
            return LeftJoinBinomial(ref c, Addtive, SyntaxType.LeftShift, SyntaxType.RightShift);
        }

        private Syntax Addtive(ref int c)
        {
            return LeftJoinBinomial(ref c, Multiplicative, SyntaxType.Add, SyntaxType.Subtract, SyntaxType.Combine);
        }

        private Syntax Multiplicative(ref int c)
        {
            return LeftJoinBinomial(ref c, Exponentive, SyntaxType.Multiply, SyntaxType.Divide, SyntaxType.Modulo);
        }

        private Syntax Exponentive(ref int c)
        {
            return LeftJoinBinomial(ref c, Primary, SyntaxType.Exponent);
        }

        private Syntax Primary(ref int c)
        {
            return Group(ref c) ?? Integer(ref c);
        }

        private Syntax Group(ref int c)
        {
            int temp = c;
            if (!CheckToken(temp, SyntaxType.LeftParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            Syntax result = Expression(ref temp);
            if (!CheckToken(temp, SyntaxType.RightParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            c = temp;
            return result;
        }

        private Syntax Integer(ref int c)
        {
            if(CheckToken(c, SyntaxType.DigitStartString))
            {
                Token t = Read(c);
                SkipSpaser(++c);
                return new NumberLiteral { Value = t.Text, Position = t.Position };
            }
            else
            {
                return null;
            }
        }
    }
}
