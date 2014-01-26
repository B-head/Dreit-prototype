using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysisOld
{
    partial class Parser
    {
        private SyntaxOld Expression(ref int c)
        {
            return Assign(ref c);
        }

        private SyntaxOld Assign(ref int c)
        {
            return RepeatParser(TokenType.Assign, ref c, Tuple, SelectToken(TokenType.LeftAssign, TokenType.OrLeftAssign, TokenType.AndLeftAssign, TokenType.XorLeftAssign,
                TokenType.LeftShiftLeftAssign, TokenType.RightShiftLeftAssign, TokenType.PlusLeftAssign, TokenType.MinusLeftAssign, TokenType.CombineLeftAssign,
                TokenType.MultiplyLeftAssign, TokenType.DivideLeftAssign, TokenType.ModuloLeftAssign, TokenType.ExponentLeftAssign,
                TokenType.RightAssign, TokenType.OrRightAssign, TokenType.AndRightAssign, TokenType.XorRightAssign,
                TokenType.LeftShiftRightAssign, TokenType.RightShiftRightAssign, TokenType.PlusRightAssign, TokenType.MinusRightAssign, TokenType.CombineRightAssign,
                TokenType.MultiplyRightAssign, TokenType.DivideRightAssign, TokenType.ModuloRightAssign, TokenType.ExponentRightAssign), Spacer, Tuple);
        }

        private SyntaxOld Tuple(ref int c)
        {
            return RepeatParser(TokenType.Tuple, ref c, PeirLiteral, SelectToken(TokenType.List), Spacer, PeirLiteral);
        }

        private SyntaxOld PeirLiteral(ref int c)
        {
            return SequenceParser(TokenType.PeirLiteral, ref c, RangeLiteral, SelectToken(TokenType.Peir), Spacer, RangeLiteral);
        }

        private SyntaxOld RangeLiteral(ref int c)
        {
            return RepeatParser(TokenType.RangeLiteral, ref c, Logical, SelectToken(TokenType.Range), Spacer, Logical);
        }

        private SyntaxOld Logical(ref int c)
        {
            return RepeatParser(TokenType.Logical, ref c, Compare, SelectToken(TokenType.Coalesce, TokenType.OrElse, TokenType.AndElse), Spacer, Compare);
        }

        private SyntaxOld Compare(ref int c)
        {
            return RepeatParser(TokenType.Compare, ref c, Bitwise, SelectToken(TokenType.Equal, TokenType.NotEqual,
                TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Incompare), Spacer, Bitwise);
        }

        private SyntaxOld Bitwise(ref int c)
        {
            return RepeatParser(TokenType.Bitwise, ref c, Shift, SelectToken(TokenType.Or, TokenType.And, TokenType.Xor), Spacer, Shift);
        }

        private SyntaxOld Shift(ref int c)
        {
            return RepeatParser(TokenType.Shift, ref c, Addtive, SelectToken(TokenType.LeftShift, TokenType.RightShift), Spacer, Addtive);
        }

        private SyntaxOld Addtive(ref int c)
        {
            return RepeatParser(TokenType.Addtive, ref c, Multiplicative, SelectToken(TokenType.Add, TokenType.Subtract, TokenType.Combine), Spacer, Multiplicative);
        }

        private SyntaxOld Multiplicative(ref int c)
        {
            return RepeatParser(TokenType.Multiplicative, ref c, Powertive, SelectToken(TokenType.Multiply, TokenType.Divide, TokenType.Modulo), Spacer, Powertive);
        }

        private SyntaxOld Powertive(ref int c)
        {
            return RepeatParser(TokenType.Powertive, ref c, Unary, SelectToken(TokenType.Exponent), Spacer, Unary);
        }

        private SyntaxOld Unary(ref int c)
        {
            return SequenceParser(TokenType.Unary, ref c, null, SelectToken(TokenType.Add, TokenType.Subtract, TokenType.Combine, TokenType.Not, TokenType.Xor), Spacer, Unary) ?? ParentAccess(ref c);
        }

        private SyntaxOld ParentAccess(ref int c)
        {
            return SequenceParser(TokenType.ParentAccess, ref c, null, SelectToken(TokenType.Access), Spacer, MenberAccess) ?? MenberAccess(ref c);
        }

        private SyntaxOld MenberAccess(ref int c)
        {
            return RepeatParser(TokenType.MenberAccess, ref c, Primary, SelectToken(TokenType.Access), Spacer, Identifier);
        }

        private SyntaxOld Primary(ref int c)
        {
            return CoalesceParser
                (ref c,
                GorupExpression,
                VariableLiteral,
                RoutineLiteral,
                LambdaLiteral,
                ClassLiteral,
                EnumLiteral,
                PragmaLiteral,
                Identifier,
                RealLiteral,
                StringLiteral
                );
        }

        private SyntaxOld GorupExpression(ref int c)
        {
            return SequenceParser(TokenType.GorupExpression, ref c, null, SelectToken(TokenType.LeftParenthesis), Spacer, Expression, SelectToken(TokenType.RightParenthesis), Spacer);
        }
    }
}
