using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace SyntacticAnalysis
{
    public partial class Lexer
    {
        private bool SinglePunctuator(ref TextPosition p)
        {
            TokenType type = TokenType.Unknoun;
            string sub = TrySubString(p.Total, 1);
            switch(sub)
            {
                case ";": type = TokenType.EndExpression; break;
                case ":": type = TokenType.Peir; break;
                case ",": type = TokenType.List; break;
                case ".": type = TokenType.Access; break;
                case "#": type = TokenType.Wild; break;
                case "@": type = TokenType.At; break;
                case "$": type = TokenType.Lambda; break;
                case "?": type = TokenType.Conditional; break;
                case "|": type = TokenType.Or; break;
                case "&": type = TokenType.And; break;
                case "^": type = TokenType.Xor; break;
                case "!": type = TokenType.Not; break;
                case "=": type = TokenType.Equal; break;
                case "<": type = TokenType.LessThan; break;
                case ">": type = TokenType.GreaterThan; break;
                case "+": type = TokenType.Add; break;
                case "-": type = TokenType.Subtract; break;
                case "~": type = TokenType.Combine; break;
                case "*": type = TokenType.Multiply; break;
                case "/": type = TokenType.Divide; break;
                case "%": type = TokenType.Modulo; break;
                case "(": type = TokenType.LeftParenthesis; break;
                case ")": type = TokenType.RightParenthesis; break;
                case "[": type = TokenType.LeftBracket; break;
                case "]": type = TokenType.RightBracket; break;
                case "{": type = TokenType.LeftBrace; break;
                case "}": type = TokenType.RightBrace; break;
                default: return false;
            }
            return TakeAddToken(ref p, 1, type);
        }

        private bool DoublePunctuator(ref TextPosition p)
        {
            TokenType type = TokenType.Unknoun;
            string sub = TrySubString(p.Total, 2);
            switch (sub)
            {
                case "::": type = TokenType.Separator; break;
                case "..": type = TokenType.Range; break;
                case "@@": type = TokenType.Pragma; break;
                case "??": type = TokenType.Coalesce; break;
                case "||": type = TokenType.OrElse; break;
                case "&&": type = TokenType.AndElse; break;
                case ":=": type = TokenType.LeftAssign; break;
                case "=:": type = TokenType.RightAssign; break;
                case "|=": type = TokenType.OrLeftAssign; break;
                case "=|": type = TokenType.OrRightAssign; break;
                case "&=": type = TokenType.AndLeftAssign; break;
                case "=&": type = TokenType.AndRightAssign; break;
                case "^=": type = TokenType.XorLeftAssign; break;
                case "=^": type = TokenType.XorRightAssign; break;
                case "==": type = TokenType.Equal; break;
                case "<>": type = TokenType.NotEqual; break;
                case "><": type = TokenType.NotEqual; break;
                case "<=": type = TokenType.LessThanOrEqual; break;
                case "=<": type = TokenType.LessThanOrEqual; break;
                case ">=": type = TokenType.GreaterThanOrEqual; break;
                case "=>": type = TokenType.GreaterThanOrEqual; break;
                case "<<": type = TokenType.LeftShift; break;
                case ">>": type = TokenType.RightShift; break;
                case "+=": type = TokenType.PlusLeftAssign; break;
                case "=+": type = TokenType.PlusRightAssign; break;
                case "-=": type = TokenType.MinusLeftAssign; break;
                case "=-": type = TokenType.MinusRightAssign; break;
                case "~=": type = TokenType.CombineLeftAssign; break;
                case "=~": type = TokenType.CombineRightAssign; break;
                case "*=": type = TokenType.MultiplyLeftAssign; break;
                case "=*": type = TokenType.MultiplyRightAssign; break;
                case "/=": type = TokenType.DivideLeftAssign; break;
                case "=/": type = TokenType.DivideRightAssign; break;
                case "%=": type = TokenType.ModuloLeftAssign; break;
                case "=%": type = TokenType.ModuloRightAssign; break;
                case "**": type = TokenType.Exponent; break;
                default: return false;
            }
            return TakeAddToken(ref p, 2, type);
        }

        private bool TriplePunctuator(ref TextPosition p)
        {
            TokenType type = TokenType.Unknoun;
            string sub = TrySubString(p.Total, 3);
            switch (sub)
            {
                case "=<>": type = TokenType.Incompare; break;
                case "=><": type = TokenType.Incompare; break;
                case "<=>": type = TokenType.Incompare; break;
                case ">=<": type = TokenType.Incompare; break;
                case "<>=": type = TokenType.Incompare; break;
                case "><=": type = TokenType.Incompare; break;
                case "<<=": type = TokenType.LeftShiftLeftAssign; break;
                case "=<<": type = TokenType.LeftShiftRightAssign; break;
                case ">>=": type = TokenType.RightShiftLeftAssign; break;
                case "=>>": type = TokenType.RightShiftRightAssign; break;
                case "**=": type = TokenType.ExponentLeftAssign; break;
                case "=**": type = TokenType.ExponentRightAssign; break;
                default: return false;
            }
            return TakeAddToken(ref p, 3, type);
        }
    }
}
