using AbstractSyntax;
using System.Collections.Generic;

namespace SyntacticAnalysis
{
    public static class Lexer
    {
        private delegate Token LexerFunction(Tokenizer t);

        public static void Lex(string text, string fileName, out List<Token> tokenList, out List<Token> errorToken, out TextPosition lastPosition)
        {
            tokenList = new List<Token>();
            errorToken = new List<Token>();
            var t = new Tokenizer(text, fileName);
            while (t.IsReadable())
            {
                LexPartion(t, tokenList, errorToken);
            }
            lastPosition = t.Position;
        }

        private static TokenType LexPartion(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            Token temp;
            temp = DisjunctionLexer
                (
                t,
                LineTerminator,
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
            return TokenType.OtherString;
        }

        private static Token DisjunctionLexer(Tokenizer t, params LexerFunction[] func)
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

        private static Token LineTerminator(Tokenizer t)
        {
            int i = 0;
            if (t.IsReadable(i) && t.MatchAny(i, "\x0A"))
            {
                i++;
                if (t.IsReadable(i) && t.MatchAny(i, "\x0D"))
                {
                    i++;
                }
            }
            else if (t.IsReadable(i) && t.MatchAny(i, "\x0D"))
            {
                i++;
                if (t.IsReadable(i) && t.MatchAny(i, "\x0A"))
                {
                    i++;
                }
            }
            return t.TakeToken(i, TokenType.LineTerminator);
        }

        private static Token WhiteSpace(Tokenizer t)
        {
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (t.MatchRange(i, '\x00', '\x20') || t.MatchAny(i, "\x7F"))
                {
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.WhiteSpace);
        }

        private static Token BlockComment(Tokenizer t)
        {
            int i = 0, nest = 1;
            if (!t.IsReadable(i + 1) || t.Read(i, 2) != "/*")
            {
                return null;
            }
            for (i = 2; !t.IsReadable(i + 1); i++)
            {
                if (t.Read(i, 2) == "*/")
                {
                    ++i;
                    if (--nest == 0)
                    {
                        break;
                    }
                }
                if (t.Read(i, 2) == "/*")
                {
                    ++i;
                    ++nest;
                }
            }
            return t.TakeToken(++i, TokenType.BlockComment);
        }

        private static Token LineCommnet(Tokenizer t)
        {
            int i = 0;
            if (!t.IsReadable(i + 1))
            {
                return null;
            }
            if (t.Read(i, 2) != "//" && t.Read(i, 2) != "#!")
            {
                return null;
            }
            for (i = 2; !t.IsReadable(i); i++)
            {
                if (t.MatchAny(i, "\x0A\x0D"))
                {
                    break;
                }
            }
            return t.TakeToken(++i, TokenType.LineCommnet);
        }

        private static bool StringLiteral(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            if (!t.IsReadable(0) || !t.MatchAny(0, "\'\"`"))
            {
                return false;
            }
            string quote = t.Read(0, 1);
            tokenList.Add(t.TakeToken(1, TokenType.QuoteSeparator));
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (t.MatchAny(i, quote))
                {
                    tokenList.Add(t.TakeToken(i, TokenType.PlainText));
                    tokenList.Add(t.TakeToken(1, TokenType.QuoteSeparator));
                    return true;
                }
                if (t.MatchAny(i, "{"))
                {
                    tokenList.Add(t.TakeToken(i, TokenType.PlainText));
                    BuiltInExpression(t, tokenList, errorToken);
                    i = -1;
                }
            }
            tokenList.Add(t.TakeToken(i, TokenType.PlainText));
            return true;
        }

        private static void BuiltInExpression(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            if (!t.IsReadable(0) || !t.MatchAny(0, "{"))
            {
                return;
            }
            var result = new List<Token>();
            int nest = 0;
            while (t.IsReadable())
            {
                var tt = LexPartion(t, tokenList, errorToken);
                if (tt == TokenType.LeftBrace)
                {
                    ++nest;
                }
                if (tt == TokenType.RightBrace)
                {
                    if (--nest == 0)
                    {
                        break;
                    }
                }
            }
        }

        private static Token LetterStartString(Tokenizer t)
        {
            int i;
            bool escape = false;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (escape && t.MatchRange(i, '!', '~'))
                {
                    escape = false;
                    continue;
                }
                if (t.MatchRange(i, 'a', 'z') || t.MatchRange(i, 'A', 'Z') || t.MatchAny(i, "_"))
                {
                    continue;
                }
                if (i > 0 && t.MatchRange(i, '0', '9'))
                {
                    continue;
                }
                if (t.MatchAny(i, "\\"))
                {
                    escape = true;
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.LetterStartString);
        }

        private static Token DigitStartString(Tokenizer t)
        {
            int i;
            bool escape = false;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (escape && t.MatchRange(i, '!', '~'))
                {
                    escape = false;
                    continue;
                }
                if (t.MatchRange(i, '0', '9') || t.MatchAny(i, "_"))
                {
                    continue;
                }
                if (i > 0 && (t.MatchRange(i, 'a', 'z') || t.MatchRange(i, 'A', 'Z')))
                {
                    continue;
                }
                if (i > 0 && t.MatchAny(i, "\\"))
                {
                    escape = true;
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.DigitStartString);
        }

        private static Token OtherString(Tokenizer t)
        {
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (!t.MatchRange(i, '\x00', '\x7F'))
                {
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.OtherString);
        }

        private static Token SinglePunctuator(Tokenizer t)
        {
            TokenType type = TokenType.Unknoun;
            string sub = t.Read(0, 1);
            switch (sub)
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
                default: return null;
            }
            return t.TakeToken(1, type);
        }

        private static Token DoublePunctuator(Tokenizer t)
        {
            TokenType type = TokenType.Unknoun;
            string sub = t.Read(0, 2);
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
                default: return null;
            }
            return t.TakeToken(2, type);
        }

        private static Token TriplePunctuator(Tokenizer t)
        {
            TokenType type = TokenType.Unknoun;
            string sub = t.Read(0, 3);
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
                default: return null;
            }
            return t.TakeToken(3, type);
        }
    }
}
