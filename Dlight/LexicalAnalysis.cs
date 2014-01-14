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
        private NumberType NumberT;

        public LexicalAnalysis(string code, string file)
        {
            Code = code + "\0";
            File = file;
            Line = 1;
            LineAdd = 0;
            TokenT = TokenType.Unknoun;
            LexerM = LexerMode.Normal;
            NumberT = NumberType.Decimal;
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
            switch(LexerM)
            {
                case LexerMode.Normal: return Normal(it);
                case LexerMode.Integer:
                    LexerM = LexerMode.RadixPoint;
                    switch(NumberT)
                    {
                        case NumberType.Binary: return BinaryNumber(it);
                        case NumberType.Octal: return OctalNumber(it);
                        case NumberType.Decimal: return DecimalNumber(it);
                        case NumberType.Hex: return HexNumber(it);
                    }
                    return null;
                case LexerMode.RadixPoint: LexerM = LexerMode.Fraction; return RadixPoint(it);
                case LexerMode.Fraction:
                    LexerM = LexerMode.ExponentPrefix;
                    switch (NumberT)
                    {
                        case NumberType.Binary: return BinaryNumber(it);
                        case NumberType.Octal: return OctalNumber(it);
                        case NumberType.Decimal: return DecimalNumber(it);
                        case NumberType.Hex: return HexNumber(it);
                    }
                    return null;
                case LexerMode.ExponentPrefix:
                    LexerM = LexerMode.Exponent;
                    switch(NumberT)
                    {
                        case NumberType.Binary: return DecimalExponent(it);
                        case NumberType.Octal: return DecimalExponent(it);
                        case NumberType.Decimal: return DecimalExponent(it);
                        case NumberType.Hex: return HexExponent(it);
                    }
                    return null;
                case LexerMode.Exponent:
                    LexerM = LexerMode.Normal;
                    switch (NumberT)
                    {
                        case NumberType.Binary: return BinaryNumber(it);
                        case NumberType.Octal: return OctalNumber(it);
                        case NumberType.Decimal: return DecimalNumber(it);
                        case NumberType.Hex: return HexNumber(it);
                    }
                    return null;
            }
            return Error(it.Next);
        }

        private int? Normal(LexicalIterator it)
        {
            char c = it.Current;
            if (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match("\\_"))
            {
                return Identifier(it.Next);
            }
            if (c.Match("\'\"`"))
            {
                TokenT = TokenType.String;
                return StringLiteral(it.Next, c);
            }
            if (c.Match("0"))
            {
                LexerM = LexerMode.Integer;
                return NumberPrefix(it.Next);
            }
            if (c.Match('1', '9'))
            {
                LexerM = LexerMode.RadixPoint;
                NumberT = NumberType.Decimal;
                return DecimalNumber(it.Next);
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
                case '{': TokenT = TokenType.LeftBrace; return 1;
                case '}': TokenT = TokenType.RightBrace; return 1;
            }
            return null;
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
            TokenT = TokenType.Identifier;
            while (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match('0', '9') || c.Match("\\_"))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
        }

        private int? StringLiteral(LexicalIterator it, char sep)
        {
            char c = it.Current, p = '\0';
            while (c != sep || p == '\\')
            {
                if (c.Match("\x00\x1A\x0A\x0D"))
                {
                    return it.Index;
                }
                p = c;
                it = it.Next;
                c = it.Current;
            }
            return it.Index + 1;
        }

        private int? NumberPrefix(LexicalIterator it)
        {
            char c = it.Current;
            switch(c)
            {
                case 'b':
                case 'B':
                    TokenT = TokenType.BinaryPrefix;
                    NumberT = NumberType.Binary;
                    return 2;
                case 'o':
                case 'O':
                    TokenT = TokenType.OctalPrefix;
                    NumberT = NumberType.Octal;
                    return 2;
                case 'd':
                case 'D':
                    TokenT = TokenType.DecimalPrefix;
                    NumberT = NumberType.Decimal;
                    return 2;
                case 'h':
                case 'H':
                    TokenT = TokenType.HexPrefix;
                    NumberT = NumberType.Hex;
                    return 2;
                case 'x':
                case 'X':
                    TokenT = TokenType.HexPrefix;
                    NumberT = NumberType.Hex;
                    return 2;
            }
            return DecimalNumber(it);
        }

        private int? BinaryNumber(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.BinaryNumber;
            while (c.Match('0', '1') || c.Match("_"))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index > 0 ? it.Index : Normal(it);
        }

        private int? OctalNumber(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.OctalNumber;
            while (c.Match('0', '7') || c.Match("_"))
            {
                it = it.Next;
                c = it.Current;
            }
            if (it.Index == 0)
            {
                LexerM = LexerMode.Normal;
            }
            return it.Index > 0 ? it.Index : Normal(it);
        }

        private int? DecimalNumber(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.DecimalNumber;
            while (c.Match('0', '9') || c.Match("_"))
            {
                it = it.Next;
                c = it.Current;
            }
            if (it.Index == 0)
            {
                LexerM = LexerMode.Normal;
            }
            return it.Index > 0 ? it.Index : Normal(it);
        }

        private int? HexNumber(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.HexNumber;
            while (c.Match('0', '9') || c.Match('a', 'f') || c.Match('A', 'F') || c.Match("_"))
            {
                it = it.Next;
                c = it.Current;
            }
            if (it.Index == 0)
            {
                LexerM = LexerMode.Normal;
            }
            return it.Index > 0 ? it.Index : Normal(it);
        }

        private int? RadixPoint(LexicalIterator it)
        {
            char c = it.Current;
            TokenT = TokenType.RadixPoint;
            switch (c)
            {
                case '.': return 1;
            }
            return Normal(it);
        }

        private int? DecimalExponent(LexicalIterator it)
        {
            char c = it.Current;
            if (c.Match("eE"))
            {
                it = it.Next;
                c = it.Current;
                switch(c)
                {
                    case '+': TokenT = TokenType.DecimalPlusExponentPrefix; return 2;
                    case '-': TokenT = TokenType.DecimalMinusExponentPrefix; return 2;
                }
                TokenT = TokenType.DecimalExponentPrefix;
                return 1;
            }
            return Normal(it);
        }

        private int? HexExponent(LexicalIterator it)
        {
            char c = it.Current;
            if (c.Match("pP"))
            {
                it = it.Next;
                c = it.Current;
                switch (c)
                {
                    case '+': TokenT = TokenType.HexPlusExponentPrefix; return 2;
                    case '-': TokenT = TokenType.HexMinusExponentPrefix; return 2;
                }
                TokenT = TokenType.HexExponentPrefix;
                return 1;
            }
            return Normal(it);
        }

        private int? BlockComment(LexicalIterator it)
        {
            char c = it.Current, p = '\0';
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
            return it.Index + 1;
        }

        private int? LinerComment(LexicalIterator it)
        {
            char c = it.Current;
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
                case '!': TokenT = TokenType.ScriptComment; return LinerComment(it.Next);
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
                case '*': TokenT = TokenType.MultiplyRightAssign; return 2;
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
            }
            return null;
        }

        private int? Divide(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': TokenT = TokenType.DivideLeftAssign; return 2;
                case '*': TokenT = TokenType.BlockComment; return BlockComment(it.Next);
                case '/': TokenT = TokenType.LineComment; return LinerComment(it.Next);
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
            ExponentPrefix,
            Exponent,
        }

        private enum NumberType
        {
            Binary,
            Octal,
            Decimal,
            Hex,
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