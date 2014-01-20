using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private AbstractSyntax LeftJoinBinomial(ref int c, ParserFunction next, params SyntaxType[] type)
        {
            AbstractSyntax left = next(ref c);
            while(IsReadable(c))
            {
                SyntaxType match;
                if(!CheckToken(c, out match, type))
                {
                    break;
                }
                SkipSpaser(++c);
                AbstractSyntax right = next(ref c);
                left = new Binomial { Left = left, Right = right, Operation = match, Position = left.Position };
            }
            return left;
        }

        private AbstractSyntax Expression(ref int c)
        {
            return Bitwise(ref c);
        }

        private AbstractSyntax Bitwise(ref int c)
        {
            return LeftJoinBinomial(ref c, Shift, SyntaxType.Or, SyntaxType.And, SyntaxType.Xor);
        }

        private AbstractSyntax Shift(ref int c)
        {
            return LeftJoinBinomial(ref c, Addtive, SyntaxType.LeftShift, SyntaxType.RightShift);
        }

        private AbstractSyntax Addtive(ref int c)
        {
            return LeftJoinBinomial(ref c, Multiplicative, SyntaxType.Plus, SyntaxType.Minus, SyntaxType.Combine);
        }

        private AbstractSyntax Multiplicative(ref int c)
        {
            return LeftJoinBinomial(ref c, Powertive, SyntaxType.Multiply, SyntaxType.Divide, SyntaxType.Modulo);
        }

        private AbstractSyntax Powertive(ref int c)
        {
            return LeftJoinBinomial(ref c, Primary, SyntaxType.Power);
        }

        private AbstractSyntax Primary(ref int c)
        {
            return Group(ref c) ?? Integer(ref c);
        }

        private AbstractSyntax Group(ref int c)
        {
            int temp = c;
            if (!CheckToken(temp, SyntaxType.LeftParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            AbstractSyntax result = Expression(ref temp);
            if (!CheckToken(temp, SyntaxType.RightParenthesis))
            {
                return null;
            }
            SkipSpaser(++temp);
            c = temp;
            return result;
        }

        private AbstractSyntax Integer(ref int c)
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
