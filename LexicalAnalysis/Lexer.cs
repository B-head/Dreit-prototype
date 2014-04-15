using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LexicalAnalysis
{
    public partial class Lexer
    {
        private delegate bool LexerFunction(ref TextPosition p);
        private string Text;
        private List<Token> Token;

        public List<Token> Lex(string text, string fileName)
        {
            Text = text;
            Token = new List<Token>();
            TextPosition p = new TextPosition { File = fileName, Total = 0, Line = 1, Row = 0 };
            while (IsEnable(p, 0))
            {
                DisjunctionLexer
                    (
                    ref p,
                    EndOfLine,
                    WhiteSpace,
                    LetterStartString,
                    DigitStartString,
                    QuadruplePunctuator,
                    TriplePunctuator,
                    DoublePunctuator,
                    SinglePunctuator,
                    OtherString
                    );
            }
            return Token;
        }

        private bool IsEnable(TextPosition p, int i)
        {
            return p.Total + i < Text.Length;
        }

        private char Peek(TextPosition p, int i)
        {
            return Text[p.Total + i];
        }

        private void DisjunctionLexer(ref TextPosition p, params LexerFunction[] func)
        {
            foreach (LexerFunction f in func)
            {
                if (f(ref p))
                {
                    break;
                }
            }
        }

        private bool TakeAddToken(ref TextPosition p, int length, TokenType type)
        {
            if (length == 0)
            {
                return false;
            }
            string text = TrySubString(p.Total, length);
            TextPosition temp = p;
            temp.Length = length;
            Token token = new Token { Text = text, Type = type, Position = temp };
            p.Total += length;
            p.Row += length;
            if (type == TokenType.LineTerminator)
            {
                p.Line++;
                p.Row = 0;
            }
            Token.Add(token);
            return true;
        }

        private string TrySubString(int startIndex, int length)
        {
            return startIndex + length <= Text.Length ? Text.Substring(startIndex, length) : string.Empty;
        }
    }

    static class LexerExtension
    {
        public static bool Match<V, T>(this V value, IEnumerable<T> list) where V : IEquatable<T>
        {
            foreach (T v in list)
            {
                if (value.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Match<V>(this V value, V stert, V end) where V : IComparable<V>
        {
            return value.CompareTo(stert) >= 0 && value.CompareTo(end) <= 0;
        }
    }
}
