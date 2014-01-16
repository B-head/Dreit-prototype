using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.LexicalAnalysis
{
    partial class Lexer
    {
        private Token SinglePunctuator(ref TextPosition p)
        {
            SyntaxType type = SyntaxType.Unknoun;
            string sub = TrySubString(p.Total, 1);
            switch(sub)
            {
                case "\'": type = SyntaxType.SingleQuote; break;
                case "\"": type = SyntaxType.DoubleQuote; break;
                case "`": type = SyntaxType.BackQuote; break;
                case ";": type = SyntaxType.EndOfExpression; break;
                case ":": type = SyntaxType.Pear; break;
                case ",": type = SyntaxType.List; break;
                case ".": type = SyntaxType.Access; break;
                case "#": type = SyntaxType.Wild; break;
                case "@": type = SyntaxType.Annotation; break;
                case "$": type = SyntaxType.Lambda; break;
                case "?": type = SyntaxType.Conditional; break;
                case "|": type = SyntaxType.Or; break;
                case "&": type = SyntaxType.And; break;
                case "^": type = SyntaxType.Xor; break;
                case "!": type = SyntaxType.Not; break;
                case "=": type = SyntaxType.Equal; break;
                case "<": type = SyntaxType.LessThan; break;
                case ">": type = SyntaxType.GreaterThan; break;
                case "+": type = SyntaxType.Plus; break;
                case "-": type = SyntaxType.Minus; break;
                case "~": type = SyntaxType.Combine; break;
                case "*": type = SyntaxType.Multiply; break;
                case "/": type = SyntaxType.Divide; break;
                case "%": type = SyntaxType.Modulo; break;
                case "(": type = SyntaxType.LeftParenthesis; break;
                case ")": type = SyntaxType.RightParenthesis; break;
                case "[": type = SyntaxType.LeftBracket; break;
                case "]": type = SyntaxType.RightBracket; break;
                case "{": type = SyntaxType.LeftBrace; break;
                case "}": type = SyntaxType.RightBrace; break;
                default: return null;
            }
            return TakeToken(ref p, 1, type);
        }

        private Token DoublePunctuator(ref TextPosition p)
        {
            SyntaxType type = SyntaxType.Unknoun;
            string sub = TrySubString(p.Total, 2);
            switch (sub)
            {
                case "/*": type = SyntaxType.StartComment; break;
                case "*/": type = SyntaxType.EndComment; break;
                case "//": type = SyntaxType.LineComment; break;
                case "#!": type = SyntaxType.LineComment; break;
                case "::": type = SyntaxType.Separator; break;
                case "..": type = SyntaxType.Range; break;
                case "@@": type = SyntaxType.Pragma; break;
                case "??": type = SyntaxType.Coalesce; break;
                case "||": type = SyntaxType.OrElse; break;
                case "&&": type = SyntaxType.AndElse; break;
                case ":=": type = SyntaxType.LeftAssign; break;
                case "=:": type = SyntaxType.RightAssign; break;
                case "|=": type = SyntaxType.OrLeftAssign; break;
                case "=|": type = SyntaxType.OrRightAssign; break;
                case "&=": type = SyntaxType.AndLeftAssign; break;
                case "=&": type = SyntaxType.AndRightAssign; break;
                case "^=": type = SyntaxType.XorLeftAssign; break;
                case "=^": type = SyntaxType.XorRightAssign; break;
                case "<>": type = SyntaxType.NotEqual; break;
                case "><": type = SyntaxType.NotEqual; break;
                case "<=": type = SyntaxType.LessThanOrEqual; break;
                case "=<": type = SyntaxType.LessThanOrEqual; break;
                case ">=": type = SyntaxType.GreaterThanOrEqual; break;
                case "=>": type = SyntaxType.GreaterThanOrEqual; break;
                case "<<": type = SyntaxType.LeftShift; break;
                case ">>": type = SyntaxType.RightShift; break;
                case "+=": type = SyntaxType.PlusLeftAssign; break;
                case "=+": type = SyntaxType.PlusRightAssign; break;
                case "-=": type = SyntaxType.MinusLeftAssign; break;
                case "=-": type = SyntaxType.MinusRightAssign; break;
                case "~=": type = SyntaxType.CombineLeftAssign; break;
                case "=~": type = SyntaxType.CombineRightAssign; break;
                case "*=": type = SyntaxType.MultiplyLeftAssign; break;
                case "=*": type = SyntaxType.MultiplyRightAssign; break;
                case "/=": type = SyntaxType.DivideLeftAssign; break;
                case "=/": type = SyntaxType.DivideRightAssign; break;
                case "%=": type = SyntaxType.ModuloLeftAssign; break;
                case "=%": type = SyntaxType.ModuloRightAssign; break;
                case "**": type = SyntaxType.Power; break;
                case "++": type = SyntaxType.Increment; break;
                case "--": type = SyntaxType.Decrement; break;
                default: return null;
            }
            return TakeToken(ref p, 2, type);
        }

        private Token TriplePunctuator(ref TextPosition p)
        {
            SyntaxType type = SyntaxType.Unknoun;
            string sub = TrySubString(p.Total, 3);
            switch (sub)
            {
                case "=<>": type = SyntaxType.Incompare; break;
                case "=><": type = SyntaxType.Incompare; break;
                case "<=>": type = SyntaxType.Incompare; break;
                case ">=<": type = SyntaxType.Incompare; break;
                case "<>=": type = SyntaxType.Incompare; break;
                case "><=": type = SyntaxType.Incompare; break;
                case "<<=": type = SyntaxType.LeftShiftLeftAssign; break;
                case "=<<": type = SyntaxType.LeftShiftRightAssign; break;
                case ">>=": type = SyntaxType.RightShiftLeftAssign; break;
                case "=>>": type = SyntaxType.RightShiftRightAssign; break;
                case "**=": type = SyntaxType.PowerLeftAssign; break;
                case "=**": type = SyntaxType.PowerRightAssign; break;
                default: return null;
            }
            return TakeToken(ref p, 3, type);
        }
    }
}
