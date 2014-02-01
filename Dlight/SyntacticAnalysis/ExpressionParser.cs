using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private ExpressionList ExpressionList(ref int c, bool root = false)
        {
            ExpressionList result = new ExpressionList();
            while (IsReadable(c))
            {
                if (CheckToken(c, TokenType.EndExpression))
                {
                    SkipSpaser(++c);
                    continue;
                }
                Element temp = Expression(ref c);
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

        private Element Expression(ref int c)
        {
            Element temp = LeftAssign(ref c);
            if (CheckToken(c, TokenType.EndExpression))
            {
                SkipSpaser(++c);
            }
            return temp;
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
