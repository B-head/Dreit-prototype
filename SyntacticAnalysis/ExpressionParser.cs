using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using Common;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private Element Expression(ref int c)
        {
            return LeftAssign(ref c);
        }

        private Element LeftAssign(ref int c)
        {
            return RightAssociative<LeftAssign>(ref c, RightAssign, TokenType.LeftAssign);
        }

        private Element RightAssign(ref int c)
        {
            return LeftAssociative<RightAssign>(ref c, TupleList, TokenType.RightAssign);
        }

        private Element TupleList(ref int c)
        {
            var tuple = ParseTuple(ref c, Addtive);
            if (tuple.Count > 1)
            {
                return tuple;
            }
            if (tuple.Count > 0)
            {
                return tuple.GetChild(0);
            }
            else
            {
                return null;
            }
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
            return RightAssociative<DyadicCalculate>(ref c, CallRoutine, TokenType.Exponent);
        }

        private Element CallRoutine(ref int c)
        {
            var access = MemberAccess(ref c);
            int temp = c;
            if (!CheckToken(temp, TokenType.LeftParenthesis))
            {
                return access;
            }
            SkipSpaser(++temp);
            var argument = ParseTuple(ref temp, Directive);
            if (!CheckToken(temp, TokenType.RightParenthesis))
            {
                return access;
            }
            SkipSpaser(++temp);
            c = temp;
            return new CallRoutine { Access = access, Argument = argument, Position = access.Position };
        }

        private Element MemberAccess(ref int c)
        {
            return LeftAssociative<MemberAccess>(ref c, Primary, TokenType.Access);
        }
    }
}
