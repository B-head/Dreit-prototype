using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class LexicalAnalysis : IEnumerable<Token>
    {
        private static readonly char[] whiteSpace = { '\u0009', '\u000B', '\u000C', '\u0020' };
        private static readonly char[] lineTerminator = { '\u0000', '\u000A', '\u000D' };
        private static readonly char[] stringSeparator = { '"', '\'', '`' };
        private static readonly string[] punctuator1 = { "!", "#", "$", "%", "&", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "[", "]", "^", "{", "|", "}", "~" };
        private static readonly string[] punctuator2 = { "!(", "++", "--", "..", "::", "<=", ">=", "!=", "||", "&&", "<<", ">>", ":=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" };
        private static readonly string[] punctuator3 = { "<<=", ">>=", "..." };

        private string current;
        private string file;
        private int line;

        public LexicalAnalysis(string text, string filename)
        {
            current = text;
            file = filename;
            line = 1;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            Func<Token?>[] lexer = new Func<Token?>[] { LexerNumber, LexerString, LexerIdentifier, LexerComment, LexerPunctuator };
            while(MoveNextToken())
            {
                Token? v = null;
                foreach(Func<Token?> f in lexer)
                {
                    v = f();
                    if(v.HasValue)
                    {
                        yield return v.Value;
                        break;
                    }
                }
                if (!v.HasValue)
                {
                    LexerError("'{0}'は使用出来ない文字です。", current[0]);
                    current = current.Substring(1);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Token LexerToken()
        {
            return LexerToken();
        }

        private bool MoveNextToken()
        {
            for(int i = 0; i < current.Length; i++)
            {
                char c = current[i];
                if(IsLineTerminator(c))
                {
                    line++;
                }
                if(!IsWhiteSpace(c) && !IsLineTerminator(c))
                {
                    current = current.Substring(i);
                    return current != string.Empty;
                }
            }
            return false;
        }

        private string TakeToken(int startIndex)
        {
            string result = current.Substring(0, startIndex);
            current = current.Substring(startIndex);
            return result;
        }

        private Token? LexerNumber()
        {
            for (int i = 0; i < current.Length; i++)
            {
                char c = current[i];
                if (!IsDigit(c))
                {
                    return i == 0 ? null : (Token?)new Token { Value = TakeToken(i), File = file, Line = line };
                }
            }
            return null;
        }

        private Token? LexerString()
        {
            char sep = current[0];
            if(!stringSeparator.Any(v => v == sep))
            {
                return null;
            }
            for (int i = 1; i < current.Length; i++)
            {
                char c = current[i];
                if (c == sep)
                {
                    return new Token { Value = TakeToken(i + 1), File = file, Line = line };
                }
            }
            LexerError("文字列リテラル中にファイルの終端に到達しました。");
            return null;
        }

        private Token? LexerComment()
        {
            if(current.Length < 2)
            {
                return null;
            }
            string temp = current.Substring(0, 2);
            if(temp == "/*")
            {
                for (int i = 2; i < current.Length; i++)
                {
                    char c1 = current[i], c2 = current[i + 1];
                    if (c1 == '*' && c2 == '/')
                    {
                        return new Token { Value = TakeToken(i + 2), File = file, Line = line };
                    }
                }
            }
            else if(temp == "//" || temp == "#!")
            {
                for (int i = 2; i < current.Length; i++)
                {
                    char c = current[i];
                    if (IsLineTerminator(c))
                    {
                        return new Token { Value = TakeToken(i), File = file, Line = line };
                    }
                }
                return new Token { Value = TakeToken(current.Length), File = file, Line = line };
            }
            return null;
        }

        private Token? LexerPunctuator()
        {
            if (current.Length >= 3)
            {
                string temp = current.Substring(0, 3);
                if (punctuator3.Any(v => v == temp))
                {
                    return new Token { Value = TakeToken(3), File = file, Line = line };
                }
            }
            if (current.Length >= 2)
            {
                string temp = current.Substring(0, 2);
                if (punctuator2.Any(v => v == temp))
                {
                    return new Token { Value = TakeToken(2), File = file, Line = line };
                }
            }
            if (current.Length >= 1)
            {
                string temp = current.Substring(0, 1);
                if (punctuator1.Any(v => v == temp))
                {
                    return new Token { Value = TakeToken(1), File = file, Line = line };
                }
            }
            return null;
        }

        private Token? LexerIdentifier()
        {
            for (int i = 0; i < current.Length; i++)
            {
                char c = current[i];
                if (!IsLetter(c) && !IsDigit(c))
                {
                    return i == 0 ? null : (Token?)new Token { Value = TakeToken(i), File = file, Line = line };
                }
            }
            return null;
        }

        private void LexerError(string format, params object[] args)
        {
            Common.CompileError(string.Format(format, args), file, line);
        }

        public static bool IsWhiteSpace(char c)
        {
            return whiteSpace.Any(v => v == c);
        }

        public static bool IsLineTerminator(char c)
        {
            return lineTerminator.Any(v => v == c);
        }

        public static bool IsDigit(char c)
        {
            if ('0' <= c && c <= '9') return true;
            return false;
        }

        public static bool IsLetter(char c)
        {
            if ('a' <= c && c <= 'z') return true;
            if ('A' <= c && c <= 'Z') return true;
            if (c == '_') return true;
            return false;
        }
    }
}
