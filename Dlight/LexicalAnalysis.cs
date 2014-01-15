using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class LexicalAnalysis : IEnumerable<Token>
    {
        private string Code;
        private string File;
        private int Line;
        private int LineAdd;
        private TokenType TokenT;
        private LexerMode LexerM;
        private char StrSep;

        public LexicalAnalysis(string code, string file)
        {
            Code = code + "\0";
            File = file;
            Line = 1;
            LineAdd = 0;
            TokenT = TokenType.Unknoun;
            LexerM = LexerMode.Normal;
            StrSep = '\0';
        }

        public IEnumerator<Token> GetEnumerator()
        {
            while (Code != string.Empty)
            {
                LexicalIterator it = new LexicalIterator(Code);
                int? length = Root(it);
                yield return TakeToken(length.GetValueOrDefault(Code.Length));
                Line += LineAdd;
                LineAdd = 0;
                TokenT = TokenType.Unknoun;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Token TakeToken(int length)
        {
            string value = Code.Substring(0, length);
            Code = Code.Substring(length);
            return new Token(value, TokenT, File, Line);
        }

        private int? Root(LexicalIterator it)
        {
            char c = it.Current;
            switch (LexerM)
            {
                case LexerMode.Normal: 
                case LexerMode.PostString:
                case LexerMode.BuiltIn:
                case LexerMode.PostBlockComment:
                    return Normal(it);
                case LexerMode.PreBuiltIn: LexerM = LexerMode.BuiltIn; return Normal(it);
                case LexerMode.String: return String(it);
                case LexerMode.Integer: LexerM = LexerMode.RadixPoint; return Number(it);
                case LexerMode.RadixPoint: LexerM = LexerMode.Fraction; return RadixPoint(it);
                case LexerMode.Fraction: LexerM = LexerMode.Normal; return Number(it);
                case LexerMode.BlockComment: LexerM = LexerMode.PostBlockComment; return BlockComment(it);
                case LexerMode.LineComment: LexerM = LexerMode.Normal; return LineComment(it);
            }
            return null;
        }

        private int? Normal(LexicalIterator it)
        {
            char c = it.Current;
            if (c.Match("\x00\x1A"))
            {
                TokenT = TokenType.EndOfFile;
                return null;
            }
            if (c.Match("\x0A\x0D"))
            {
                TokenT = TokenType.EndOfLine;
                return EndOfLine(it.Next, c);
            }
            if (c.Match('\x00', '\x20') || c.Match("\x7F"))
            {
                TokenT = TokenType.Space;
                return Space(it.Next);
            }
            if (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match("\\_"))
            {
                TokenT = TokenType.Identifier;
                return Identifier(it.Next);
            }
            if (c.Match("\'\"`"))
            {
                switch (LexerM)
                {
                    case LexerMode.BuiltIn:
                    case LexerMode.PostString:
                        LexerM = LexerMode.Normal; 
                        TokenT = TokenType.EndString; 
                        return 1;
                    default:
                        LexerM = LexerMode.String;
                        TokenT = TokenType.StartString;
                        StrSep = c;
                        return 1;
                }
            }
            if (c.Match('0', '9'))
            {
                LexerM = LexerMode.Integer;
                return NumberPrefix(it.Next);
            }
            switch (c)
            {
                case ';': TokenT = TokenType.EndOfExpression; return 1;
                case ':': TokenT = TokenType.Pear; return Pear(it.Next) ?? 1;
                case ',': TokenT = TokenType.List; return 1;
                case '.': TokenT = TokenType.Access; return Access(it.Next) ?? 1;
                case '#': TokenT = TokenType.Wild; return Wild(it.Next) ?? 1;
                case '@': TokenT = TokenType.Annotation; return Annotation(it.Next) ?? 1;
                case '$': TokenT = TokenType.Lambda; return 1;
                case '?': TokenT = TokenType.Conditional; return Conditional(it.Next) ?? 1;
                case '|': TokenT = TokenType.Or; return Or(it.Next) ?? 1;
                case '&': TokenT = TokenType.And; return And(it.Next) ?? 1;
                case '^': TokenT = TokenType.Xor; return Xor(it.Next) ?? 1;
                case '!': TokenT = TokenType.Not; return 1;
                case '=': TokenT = TokenType.Equal; return Equal(it.Next) ?? 1;
                case '<': TokenT = TokenType.LessThan; return LessThan(it.Next) ?? 1;
                case '>': TokenT = TokenType.GreaterThan; return GreaterThan(it.Next) ?? 1;
                case '+': TokenT = TokenType.Plus; return Plus(it.Next) ?? 1;
                case '-': TokenT = TokenType.Minus; return Minus(it.Next) ?? 1;
                case '~': TokenT = TokenType.Combine; return Combine(it.Next) ?? 1;
                case '*': TokenT = TokenType.Multiply; return Multiply(it.Next) ?? 1;
                case '/': TokenT = TokenType.Divide; return Divide(it.Next) ?? 1;
                case '%': TokenT = TokenType.Modulo; return Modulo(it.Next) ?? 1;
                case '(': TokenT = TokenType.LeftParenthesis; return 1;
                case ')': TokenT = TokenType.RightParenthesis; return 1;
                case '[': TokenT = TokenType.LeftBracket; return 1;
                case ']': TokenT = TokenType.RightBracket; return 1;
                case '{': TokenT = (LexerM == LexerMode.BuiltIn ? TokenType.StartBuiltIn : TokenType.LeftBrace); return 1;
                case '}': TokenT = (LexerM == LexerMode.BuiltIn ? TokenType.EndBuiltIn : TokenType.RightBrace); return 1;
            }
            return Error(it.Next);
        }

        private int? EndOfLine(LexicalIterator it, char prev)
        {
            char c = it.Current;
            LineAdd++;
            switch (prev)
            {
                case '\u000A': return c == '\u000D' ? 2 : 1;
                case '\u000D': return c == '\u000A' ? 2 : 1;
            }
            return null;
        }

        private int? Space(LexicalIterator it)
        {
            char c = it.Current;
            while (c.Match('\x00', '\x20') || c == '\x7F')
            {
                if (c.Match("\x00\x1A\x0A\x0D"))
                {
                    return it.Index;
                }
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? Identifier(LexicalIterator it)
        {
            char c = it.Current;
            while (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match('0', '9') || c.Match("\\_"))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? String(LexicalIterator it)
        {
            char c = it.Current, p = '\0';
            TokenT = TokenType.String;
            while (c != StrSep || p == '\\')
            {
                if (c == '{' && p != '\\')
                {
                    LexerM = LexerMode.PreBuiltIn;
                    return it.Index;
                }
                if (c.Match("\x00\x1A"))
                {
                    break;
                }
                if (c.Match("\x0A\x0D"))
                {
                    if (EndOfLine(it.Next, c).GetValueOrDefault() == 2)
                    {
                        it = it.Next;
                        c = it.Current;
                    }
                }
                p = c;
                it = it.Next;
                c = it.Current;
            }
            LexerM = LexerMode.PostString;
            return it.Index;
        }

        private int? NumberPrefix(LexicalIterator it)
        {
            char c = it.Current;
            switch(c)
            {
                case 'b':
                case 'B':
                    TokenT = TokenType.BinaryPrefix;
                    return 2;
                case 'o':
                case 'O':
                    TokenT = TokenType.OctalPrefix;
                    return 2;
                case 'd':
                case 'D':
                    TokenT = TokenType.DecimalPrefix;
                    return 2;
                case 'h':
                case 'H':
                    TokenT = TokenType.HexPrefix;
                    return 2;
                case 'x':
                case 'X':
                    TokenT = TokenType.HexPrefix;
                    return 2;
            }
            LexerM = LexerMode.RadixPoint;
            return Number(it);
        }

        private int? Number(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.Number;
            while (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match('0', '9') || c.Match("_"))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? RadixPoint(LexicalIterator it)
        {
            char c1 = it.Current, c2 = it.Next.Current;
            TokenT = TokenType.RadixPoint;
            if(c1 == '.' && c2 != '.')
            {
                return 1;
            }
            return Normal(it);
        }

        private int? BlockComment(LexicalIterator it)
        {
            char c = it.Current, p = '\0';
            TokenT = TokenType.Comment;
            while (c != '/' || p != '*')
            {
                if (c.Match("\x00\x1A"))
                {
                    return it.Index;
                }
                if (c.Match("\x0A\x0D"))
                {
                    if(EndOfLine(it.Next, c).GetValueOrDefault() == 2)
                    {
                        it = it.Next;
                        c = it.Current;
                    }
                }
                p = c;
                it = it.Next;
                c = it.Current;
            }
            return it.Index - 1;
        }

        private int? LineComment(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.Comment;
            while (!c.Match("\x00\x1A\x0A\x0D"))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? Error(LexicalIterator it)
        {
            char c = it.Current;
            while (!c.Match('\x00', '\x7F'))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? Pear(LexicalIterator it)
        {
            char c = it.Current;
            switch(c)
            {
                case ':': TokenT = TokenType.Separator; return 2;
                case '=': TokenT = TokenType.LeftAssign; return 2;
            }
            return null;
        }

        private int? Access(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '.': TokenT = TokenType.Range; return 2;
            }
            return null;
        }

        private int? Wild(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '!': TokenT = TokenType.PreLineComment; LexerM = LexerMode.LineComment; return 2;
            }
            return null;
        }

        private int? Annotation(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '@': TokenT = TokenType.Pragma; return 2;
            }
            return null;
        }

        private int? Conditional(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '?': TokenT = TokenType.Coalesce; return 2;
            }
            return null;
        }

        private int? Or(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '|': TokenT = TokenType.OrElse; return 2;
                case '=': TokenT = TokenType.OrLeftAssign; return 2;
            }
            return null;
        }

        private int? And(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '&': TokenT = TokenType.AndElse; return 2;
                case '=': TokenT = TokenType.AndLeftAssign; return 2;
            }
            return null;
        }

        private int? Xor(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.XorLeftAssign; return 2;
            }
            return null;
        }

        private int? Equal(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case ':': TokenT = TokenType.RightAssign; return 2;
                case '|': TokenT = TokenType.OrRightAssign; return 2;
                case '&': TokenT = TokenType.AndRightAssign; return 2;
                case '^': TokenT = TokenType.XorRightAssign; return 2;
                case '+': TokenT = TokenType.PlusRightAssign; return 2;
                case '-': TokenT = TokenType.MinusRightAssign; return 2;
                case '~': TokenT = TokenType.CombineRightAssign; return 2;
                case '*': TokenT = TokenType.MultiplyRightAssign; return Equal4(it.Next) ?? 2;
                case '/': TokenT = TokenType.DivideRightAssign; return 2;
                case '%': TokenT = TokenType.ModuloRightAssign; return 2;
                case '<': TokenT = TokenType.LessThanOrEqual; return Equal2(it.Next) ?? 2;
                case '>': TokenT = TokenType.GreaterThanOrEqual; return Equal3(it.Next) ?? 2;
            }
            return null;
        }

        private int? Equal2(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '<': TokenT = TokenType.LeftShiftRightAssign; return 3;
            }
            return Incompare(it, '>') ?? null;
        }

        private int? Equal3(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '>': TokenT = TokenType.RightShiftRightAssign; return 3;
            }
            return Incompare(it, '<') ?? null;
        }

        private int? Equal4(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '*': TokenT = TokenType.PowerRightAssign; return 3;
            }
            return null;
        }

        private int? LessThan(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.LessThanOrEqual; return Incompare(it.Next, '>') ?? 2;
                case '<': TokenT = TokenType.LeftShift; return LeftShift(it.Next) ?? 2;
                case '>': TokenT = TokenType.NotEqual; return Incompare(it.Next, '=') ?? 2;
            }
            return null;
        }

        private int? LeftShift(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.LeftShiftLeftAssign; return 3;
            }
            return null;
        }

        private int? GreaterThan(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.GreaterThanOrEqual; return Incompare(it.Next, '<') ?? 2;
                case '<': TokenT = TokenType.NotEqual; return Incompare(it.Next, '=') ?? 2;
                case '>': TokenT = TokenType.RightShift; return RightShift(it.Next) ?? 2;
            }
            return null;
        }

        private int? RightShift(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.RightShiftLeftAssign; return 3;
            }
            return null;
        }

        private int? Incompare(LexicalIterator it, char need)
        {
            char c = it.Current;
            if(c == need)
            {
                TokenT = TokenType.Incompare;
                return 3;
            }
            return null;
        }

        private int? Plus(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.PlusLeftAssign; return 2;
                case '+': TokenT = TokenType.Increment; return 2;
            }
            return null;
        }

        private int? Minus(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.MinusLeftAssign; return 2;
                case '-': TokenT = TokenType.Decrement; return 2;
            }
            return null;
        }

        private int? Combine(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.CombineLeftAssign; return 2;
            }
            return null;
        }

        private int? Multiply(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.MultiplyLeftAssign; return 2;
                case '+': TokenT = TokenType.Power; return Power(it.Next) ?? 2;
                case '/': TokenT = TokenType.EndBlockComment; LexerM = LexerMode.Normal; return 2;
            }
            return null;
        }

        private int? Power(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.PowerLeftAssign; return 3;
            }
            return null;
        }

        private int? Divide(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.DivideLeftAssign; return 2;
                case '*': TokenT = TokenType.StartBlockComment; LexerM = LexerMode.BlockComment; return 2;
                case '/': TokenT = TokenType.PreLineComment; LexerM = LexerMode.LineComment; return 2;
            }
            return null;
        }

        private int? Modulo(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.ModuloLeftAssign; return 2;
            }
            return null;
        }

        private enum LexerMode
        {
            Normal,
            Integer,
            RadixPoint,
            Fraction,
            String,
            PostString,
            PreBuiltIn,
            BuiltIn,
            BlockComment,
            PostBlockComment,
            LineComment,
        }

        private struct LexicalIterator
        {
            public readonly string Value;
            public readonly int Index;

            public LexicalIterator(string value, int index = 0)
            {
                Value = value;
                Index = index;
            }

            public LexicalIterator Next
            {
                get { return new LexicalIterator(Value, Index + 1); }
            }

            public char Current
            {
                get { return Value[Index]; }
            }
        }
    }
}