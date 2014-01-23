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
            return RepeatParser(SyntaxType.Assign, ref c, Tuple, SelectToken(SyntaxType.LeftAssign, SyntaxType.OrLeftAssign, SyntaxType.AndLeftAssign, SyntaxType.XorLeftAssign,
                SyntaxType.LeftShiftLeftAssign, SyntaxType.RightShiftLeftAssign, SyntaxType.PlusLeftAssign, SyntaxType.MinusLeftAssign, SyntaxType.CombineLeftAssign,
                SyntaxType.MultiplyLeftAssign, SyntaxType.DivideLeftAssign, SyntaxType.ModuloLeftAssign, SyntaxType.ExponentLeftAssign,
                SyntaxType.RightAssign, SyntaxType.OrRightAssign, SyntaxType.AndRightAssign, SyntaxType.XorRightAssign,
                SyntaxType.LeftShiftRightAssign, SyntaxType.RightShiftRightAssign, SyntaxType.PlusRightAssign, SyntaxType.MinusRightAssign, SyntaxType.CombineRightAssign,
                SyntaxType.MultiplyRightAssign, SyntaxType.DivideRightAssign, SyntaxType.ModuloRightAssign, SyntaxType.ExponentRightAssign), Spacer, Tuple);
        }

        private SyntaxOld Tuple(ref int c)
        {
            return RepeatParser(SyntaxType.Tuple, ref c, PeirLiteral, SelectToken(SyntaxType.List), Spacer, PeirLiteral);
        }

        private SyntaxOld PeirLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.PeirLiteral, ref c, RangeLiteral, SelectToken(SyntaxType.Peir), Spacer, RangeLiteral);
        }

        private SyntaxOld RangeLiteral(ref int c)
        {
            return RepeatParser(SyntaxType.RangeLiteral, ref c, Logical, SelectToken(SyntaxType.Range), Spacer, Logical);
        }

        private SyntaxOld Logical(ref int c)
        {
            return RepeatParser(SyntaxType.Logical, ref c, Compare, SelectToken(SyntaxType.Coalesce, SyntaxType.OrElse, SyntaxType.AndElse), Spacer, Compare);
        }

        private SyntaxOld Compare(ref int c)
        {
            return RepeatParser(SyntaxType.Compare, ref c, Bitwise, SelectToken(SyntaxType.Equal, SyntaxType.NotEqual,
                SyntaxType.LessThan, SyntaxType.LessThanOrEqual, SyntaxType.GreaterThan, SyntaxType.GreaterThanOrEqual, SyntaxType.Incompare), Spacer, Bitwise);
        }

        private SyntaxOld Bitwise(ref int c)
        {
            return RepeatParser(SyntaxType.Bitwise, ref c, Shift, SelectToken(SyntaxType.Or, SyntaxType.And, SyntaxType.Xor), Spacer, Shift);
        }

        private SyntaxOld Shift(ref int c)
        {
            return RepeatParser(SyntaxType.Shift, ref c, Addtive, SelectToken(SyntaxType.LeftShift, SyntaxType.RightShift), Spacer, Addtive);
        }

        private SyntaxOld Addtive(ref int c)
        {
            return RepeatParser(SyntaxType.Addtive, ref c, Multiplicative, SelectToken(SyntaxType.Add, SyntaxType.Subtract, SyntaxType.Combine), Spacer, Multiplicative);
        }

        private SyntaxOld Multiplicative(ref int c)
        {
            return RepeatParser(SyntaxType.Multiplicative, ref c, Powertive, SelectToken(SyntaxType.Multiply, SyntaxType.Divide, SyntaxType.Modulo), Spacer, Powertive);
        }

        private SyntaxOld Powertive(ref int c)
        {
            return RepeatParser(SyntaxType.Powertive, ref c, Unary, SelectToken(SyntaxType.Exponent), Spacer, Unary);
        }

        private SyntaxOld Unary(ref int c)
        {
            return SequenceParser(SyntaxType.Unary, ref c, null, SelectToken(SyntaxType.Add, SyntaxType.Subtract, SyntaxType.Combine, SyntaxType.Not, SyntaxType.Xor), Spacer, Unary) ?? ParentAccess(ref c);
        }

        private SyntaxOld ParentAccess(ref int c)
        {
            return SequenceParser(SyntaxType.ParentAccess, ref c, null, SelectToken(SyntaxType.Access), Spacer, MenberAccess) ?? MenberAccess(ref c);
        }

        private SyntaxOld MenberAccess(ref int c)
        {
            return RepeatParser(SyntaxType.MenberAccess, ref c, Primary, SelectToken(SyntaxType.Access), Spacer, Identifier);
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
            return SequenceParser(SyntaxType.GorupExpression, ref c, null, SelectToken(SyntaxType.LeftParenthesis), Spacer, Expression, SelectToken(SyntaxType.RightParenthesis), Spacer);
        }
    }
}
