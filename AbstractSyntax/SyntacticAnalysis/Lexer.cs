/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AbstractSyntax;
using System.Collections.Generic;

namespace AbstractSyntax.SyntacticAnalysis
{
    public static class Lexer
    {
        internal delegate Token LexerFunction(Tokenizer t);

        public static TokenCollection Lex(string text, string fileName)
        {
            var tokenList = new List<Token>();
            var errorToken = new List<Token>();
            var t = new Tokenizer(text, fileName);
            while (t.IsReadable())
            {
                LexPartion(t, tokenList, errorToken);
            }
            return new TokenCollection(text, fileName, tokenList, errorToken, t.Position);
        }

        private static TokenType LexPartion(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            Token temp = LineTerminator(t);
            if(temp)
            {
                tokenList.Add(temp);
                return temp.TokenType;
            }
            temp = DisjunctionLexer
                (
                t,
                WhiteSpace,
                BlockComment,
                LineCommnet
                );
            if (temp)
            {
                return temp.TokenType;
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
            if (temp)
            {
                tokenList.Add(temp);
                return temp.TokenType;
            }
            errorToken.Add(OtherString(t));
            return TokenType.OtherString;
        }

        private static Token DisjunctionLexer(Tokenizer t, params LexerFunction[] func)
        {
            foreach (var f in func)
            {
                var token = f(t);
                if (token)
                {
                    return token;
                }
            }
            return Token.Empty;
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
                if (t.MatchRange(i, '\x00', '\x09') || t.MatchRange(i, '\x0B', '\x0C') || t.MatchRange(i, '\x0E', '\x20') || t.MatchAny(i, "\x7F"))
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
                return Token.Empty;
            }
            for (i = 2; t.IsReadable(i + 1); i++)
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
                return Token.Empty;
            }
            if (t.Read(i, 2) != "//" && t.Read(i, 2) != "#!")
            {
                return Token.Empty;
            }
            for (i = 2; t.IsReadable(i); i++)
            {
                if (t.MatchAny(i, "\x0A\x0D"))
                {
                    break;
                }
            }
            return t.TakeToken(i, TokenType.LineCommnet);
        }

        private static bool StringLiteral(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            if (!t.IsReadable(0) || !t.MatchAny(0, "\'\"`#"))
            {
                return false;
            }
            string quote = t.Read(0, 1);
            bool isEfficient = false;
            if (quote == "#")
            {
                if (!t.IsReadable(1) || !t.MatchAny(1, "\'\"`"))
                {
                    return false;
                }
                quote = t.Read(1, 1);
                tokenList.Add(t.TakeToken(2, TokenType.EfficientQuoteSeparator));
                isEfficient = true;
            }
            else
            {
                tokenList.Add(t.TakeToken(1, TokenType.QuoteSeparator));
                isEfficient = false;
            }
            bool escape = false;
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (!escape && t.MatchAny(i, quote))
                {
                    if (i > 0)
                    {
                        tokenList.Add(t.TakeToken(i, TokenType.PlainText));
                    }
                    tokenList.Add(t.TakeToken(1, TokenType.QuoteSeparator));
                    return true;
                }
                if (!escape && isEfficient && t.MatchAny(i, "{"))
                {
                    if (i > 0)
                    {
                        tokenList.Add(t.TakeToken(i, TokenType.PlainText));
                    }
                    BuiltInExpression(t, tokenList, errorToken);
                    i = -1;
                }
                else if (isEfficient && t.MatchAny(i, "\\"))
                {
                    escape = !escape;
                    continue;
                }
                escape = false;
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
                    escape = false;
                    continue;
                }
                if (i > 0 && t.MatchRange(i, '0', '9'))
                {
                    escape = false;
                    continue;
                }
                if (t.MatchAny(i, "\\"))
                {
                    escape = !escape;
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.LetterStartString);
        }

        private static Token DigitStartString(Tokenizer t)
        {
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (t.MatchRange(i, '0', '9'))
                {
                    continue;
                }
                if (i > 0 && (t.MatchRange(i, 'a', 'z') || t.MatchRange(i, 'A', 'Z') || t.MatchAny(i, "_")))
                {
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
                case ":": type = TokenType.Pair; break;
                case ",": type = TokenType.List; break;
                case ".": type = TokenType.Access; break;
                case "#": type = TokenType.Zone; break;
                case "@": type = TokenType.Attribute; break;
                case "$": type = TokenType.Lambda; break;
                case "?": type = TokenType.Reject; break;
                case "!": type = TokenType.Template; break;
                case "|": type = TokenType.Typeof; break;
                case "&": type = TokenType.Refer; break;
                case "=": type = TokenType.Equal; break;
                case "<": type = TokenType.LessThan; break;
                case ">": type = TokenType.GreaterThan; break;
                case "~": type = TokenType.Join; break;
                case "+": type = TokenType.Add; break;
                case "-": type = TokenType.Subtract; break;
                case "*": type = TokenType.Multiply; break;
                case "/": type = TokenType.Divide; break;
                case "%": type = TokenType.Modulo; break;
                case "(": type = TokenType.LeftParenthesis; break;
                case ")": type = TokenType.RightParenthesis; break;
                case "[": type = TokenType.LeftBracket; break;
                case "]": type = TokenType.RightBracket; break;
                case "{": type = TokenType.LeftBrace; break;
                case "}": type = TokenType.RightBrace; break;
                default: return Token.Empty;
            }
            return t.TakeToken(1, type);
        }

        private static Token DoublePunctuator(Tokenizer t)
        {
            TokenType type = TokenType.Unknoun;
            string sub = t.Read(0, 2);
            switch (sub)
            {
                case "->": type = TokenType.ReturnArrow; break;
                case "::": type = TokenType.Separator; break;
                case "..": type = TokenType.Range; break;
                case "@@": type = TokenType.Pragma; break;
                case "##": type = TokenType.Macro; break;
                case "??": type = TokenType.Nullable; break;
                case "||": type = TokenType.Or; break;
                case "&&": type = TokenType.And; break;
                case "!!": type = TokenType.Not; break;
                case "++": type = TokenType.Plus; break;
                case "--": type = TokenType.Minus; break;
                case "==": type = TokenType.Equal; break;
                case "<>": type = TokenType.NotEqual; break;
                case "><": type = TokenType.NotEqual; break;
                case "<=": type = TokenType.LessThanOrEqual; break;
                case "=<": type = TokenType.LessThanOrEqual; break;
                case ">=": type = TokenType.GreaterThanOrEqual; break;
                case "=>": type = TokenType.GreaterThanOrEqual; break;
                case "<<": type = TokenType.LeftCompose; break;
                case ">>": type = TokenType.RightCompose; break;
                case ":=": type = TokenType.LeftPipeline; break;
                case "=:": type = TokenType.RightPipeline; break;
                case "+=": type = TokenType.Add | TokenType.LeftPipeline; break;
                case "=+": type = TokenType.Add | TokenType.RightPipeline; break;
                case "-=": type = TokenType.Subtract | TokenType.LeftPipeline; break;
                case "=-": type = TokenType.Subtract | TokenType.RightPipeline; break;
                case "*=": type = TokenType.Multiply | TokenType.LeftPipeline; break;
                case "=*": type = TokenType.Multiply | TokenType.RightPipeline; break;
                case "/=": type = TokenType.Divide | TokenType.LeftPipeline; break;
                case "=/": type = TokenType.Divide | TokenType.RightPipeline; break;
                case "%=": type = TokenType.Modulo | TokenType.LeftPipeline; break;
                case "=%": type = TokenType.Modulo | TokenType.RightPipeline; break;
                default: return Token.Empty;
            }
            return t.TakeToken(2, type);
        }

        private static Token TriplePunctuator(Tokenizer t)
        {
            TokenType type = TokenType.Unknoun;
            string sub = t.Read(0, 3);
            switch (sub)
            {
                case ":=:": type = TokenType.Swap; break;
                case "=<>": type = TokenType.Incomparable; break;
                case "=><": type = TokenType.Incomparable; break;
                case "<=>": type = TokenType.Incomparable; break;
                case ">=<": type = TokenType.Incomparable; break;
                case "<>=": type = TokenType.Incomparable; break;
                case "><=": type = TokenType.Incomparable; break;
                case "<<=": type = TokenType.LeftCompose | TokenType.LeftPipeline; break;
                case "=<<": type = TokenType.LeftCompose | TokenType.RightPipeline; break;
                case ">>=": type = TokenType.RightCompose | TokenType.LeftPipeline; break;
                case "=>>": type = TokenType.RightCompose | TokenType.RightPipeline; break;
                default: return Token.Empty;
            }
            return t.TakeToken(3, type);
        }
    }
}
