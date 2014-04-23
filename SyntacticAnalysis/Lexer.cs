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
        private delegate Token LexerFunction(Tokenizer t);
        private delegate bool LexerFunctionOld(ref TextPosition p);
        public string Text { get; private set; }
        public string FileName { get; private set; }
        public TextPosition LastPosition { get; private set; }
        private List<Token> _Token;
        public IReadOnlyList<Token> Token
        {
            get { return _Token; }
        }
        private List<Token> _ErrorToken;
        public IReadOnlyList<Token> ErrorToken
        {
            get { return _ErrorToken; }
        }

        public void Lex(string text, string fileName)
        {
            Text = text;
            FileName = fileName;
            var tokenList = new List<Token>();
            var errorToken = new List<Token>();
            var t = new Tokenizer(text, fileName);
            while (t.IsReadable())
            {
                LexPartion(t, tokenList, errorToken);
            }
            LastPosition = t.Position;
        }

        private TokenType LexPartion(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            Token temp;
            temp = DisjunctionLexer
                (
                t,
                EndOfLine,
                WhiteSpace,
                BlockComment,
                LineCommnet
                );
            if (temp != null)
            {
                return temp.Type;
            }
            if (StringLiteral(t, tokenList, errorToken))
            {
                return TokenType.PlainText;
            }
            temp = DisjunctionLexer
                (
                t,
                TriplePunctuator,
                DoublePunctuator,
                SinglePunctuator,
                LetterStartString,
                DigitStartString
                );
            if (temp != null)
            {
                tokenList.Add(temp);
                return temp.Type;
            }
            errorToken.Add(OtherString(t));
        }

        private bool IsEnable(TextPosition p, int i)
        {
            return p.Total + i < Text.Length;
        }

        private char Peek(TextPosition p, int i)
        {
            return Text[p.Total + i];
        }

        private Token DisjunctionLexer(Tokenizer t, params LexerFunction[] func)
        {
            foreach (var f in func)
            {
                var token = f(t);
                if (token != null)
                {
                    return token;
                }
            }
            return null;
        }

        private void DisjunctionLexer(ref TextPosition p, params LexerFunctionOld[] func)
        {
            foreach (LexerFunctionOld f in func)
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
            Token token = TakeToken(ref p, length, type);
            _Token.Add(token);
            return true;
        }

        private bool TakeAddErrorToken(ref TextPosition p, int length, TokenType type)
        {
            if (length == 0)
            {
                return false;
            }
            Token token = TakeToken(ref p, length, type);
            _ErrorToken.Add(token);
            return true;
        }

        private Token TakeToken(ref TextPosition p, int length, TokenType type)
        {
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
            return token;
        }

        private bool SkipToken(ref TextPosition p, int length)
        {
            if (length == 0)
            {
                return false;
            }
            p.Total += length;
            p.Row += length;
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
