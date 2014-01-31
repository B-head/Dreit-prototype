using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Element LeftAssociative<R>(ref int c, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(ref c);
            TokenType match;
            while (CheckToken(c, out match, type))
            {
                SkipSpaser(++c);
                Element right = next(ref c);
                left = new R { Left = left, Right = right, Operation = match, Position = left.Position };
            }
            return left;
        }

        private Element RightAssociative<R>(ref int c, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(ref c);
            TokenType match;
            if (!CheckToken(c, out match, type))
            {
                return left;
            }
            SkipSpaser(++c);
            Element right = RightAssociative<R>(ref c, next, type);
            return new R { Left = left, Right = right, Operation = match, Position = left.Position };
        }

        private ExpressionList Expression(ref int c, bool root = false)
        {
            ExpressionList result = new ExpressionList();
            while (IsReadable(c))
            {
                Element temp = LeftAssign(ref c);
                if (CheckToken(c, TokenType.EndExpression))
                {
                    SkipSpaser(++c);
                }
                if (temp == null)
                {
                    if(!root && CheckToken(c, TokenType.RightBrace))
                    {
                        break;
                    }
                    SkipError(c);
                    continue;
                }
                result.Append(temp);
            }
            return result;
        }

        private Element LeftAssign(ref int c)
        {
            return RightAssociative<LeftAssign>(ref c, RightAssign, TokenType.LeftAssign);
        }

        private Element RightAssign(ref int c)
        {
            return LeftAssociative<RightAssign>(ref c, Addtive, TokenType.RightAssign);
        }

        private Element Addtive(ref int c)
        {
            return LeftAssociative<DyadicCalculate>(ref c, Multiplicative, TokenType.Add, TokenType.Subtract, TokenType.Combine);
        }

        private Element Multiplicative(ref int c)
        {
            return LeftAssociative<DyadicCalculate>(ref c, Exponentive, TokenType.Multiply, TokenType.Divide, TokenType.Modulo);
        }

        private Element Exponentive(ref int c)
        {
            return RightAssociative<DyadicCalculate>(ref c, Primary, TokenType.Exponent);
        }
    }
}
