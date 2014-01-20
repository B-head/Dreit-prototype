using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.LexicalAnalysis
{
    partial class Lexer
    {
        private delegate Token LexerFunction(ref TextPosition p);
        private string Text;

        public List<Token> Lex(string text, string file)
        {
            Text = text;
            TextPosition p = new TextPosition { File = file, Total = 0, Line = 1, Row = 0 };
            List<Token> result = new List<Token>();
            while (IsEnable(p, 0))
            {
                Token token = CoalesceLexer
                    (
                    ref p,
                    EndOfLine,
                    WhiteSpace,
                    LetterStartString,
                    DigitStartString,
                    TriplePunctuator,
                    DoublePunctuator,
                    SinglePunctuator,
                    OtherString
                    );
                result.Add(token);
            }
            return result;
        }

        private bool IsEnable(TextPosition p, int i)
        {
            return p.Total + i < Text.Length;
        }

        private char Peek(TextPosition p, int i)
        {
            return Text[p.Total + i];
        }

        private Token CoalesceLexer(ref TextPosition p, params LexerFunction[] func)
        {
            Token result = null;
            foreach (LexerFunction f in func)
            {
                result = f(ref p);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        private Token TakeToken(ref TextPosition p, int length, SyntaxType type)
        {
            if (length == 0)
            {
                return null;
            }
            string text = TrySubString(p.Total, length);
            TextPosition temp = p;
            temp.Length = length;
            Token result = new Token { Text = text, Type = type, Position = temp };
            p.Total += length;
            p.Row += length;
            if (type == SyntaxType.EndLine)
            {
                p.Line++;
                p.Row = 0;
            }
            return result;
        }

        private string TrySubString(int startIndex, int length)
        {
            return startIndex + length <= Text.Length ? Text.Substring(startIndex, length) : string.Empty;
        }
    }
}
