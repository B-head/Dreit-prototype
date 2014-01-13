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
        private TokenType Type;

        public LexicalAnalysis(string code, string file)
        {
            Code = code + "\0";
            File = file;
            Line = 1;
            LineAdd = 0;
            Type = TokenType.Unknoun;
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
                Type = TokenType.Unknoun;
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
            return new Token(value, Type, File, Line);
        }

        private int? Root(LexicalIterator it)
        {
            char c = it.Current;
            if (c.Match("\x00\x1A"))
            {
                Type = TokenType.EndOfFile;
                return null;
            }
            if (c.Match("\x0A\x0D"))
            {
                Type = TokenType.EndOfLine;
                return EndOfLine(it.Next, c);
            }
            if (c.Match('\x00', '\x20') || c.Match("\\\x7F"))
            {
                Type = TokenType.Space;
                return Space(it.Next);
            }
            if (c.Match('a', 'z') || c.Match('A', 'Z') || c == '_')
            {
                Type = TokenType.Identifier;
                return Identifier(it.Next);
            }
            if (c.Match("\'\"`"))
            {
                Type = TokenType.String;
                return StringLiteral(it.Next, c);
            }
            if (c.Match('0', '9'))
            {
                Type = TokenType.Number;
                return NumberLiteral(it.Next);
            }
            switch(c)
            {
                case ';': Type = TokenType.EndOfExpression; return 1;
                case ':': Type = TokenType.Pear; return Pear(it.Next) ?? 1;
                case ',': Type = TokenType.List; return 1;
                case '.': Type = TokenType.Access; return Access(it.Next) ?? 1;
                case '#': Type = TokenType.Wild; return Wild(it.Next) ?? 1;
                case '@': Type = TokenType.Annotation; return Annotation(it.Next) ?? 1;
                case '$': Type = TokenType.Lambda; return 1;
                case '?': Type = TokenType.Conditional; return Conditional(it.Next) ?? 1;
                case '|': Type = TokenType.Or; return Or(it.Next) ?? 1;
                case '&': Type = TokenType.And; return And(it.Next) ?? 1;
                case '^': Type = TokenType.Xor; return Xor(it.Next) ?? 1;
                case '!': Type = TokenType.Not; return 1;
                case '=': Type = TokenType.Equal; return Equal(it.Next) ?? 1;
                case '<': Type = TokenType.LessThan; return LessThan(it.Next) ?? 1;
                case '>': Type = TokenType.GreaterThan; return GreaterThan(it.Next) ?? 1;
                case '+': Type = TokenType.Plus; return Plus(it.Next) ?? 1;
                case '-': Type = TokenType.Minus; return Minus(it.Next) ?? 1;
                case '~': Type = TokenType.Combine; return Combine(it.Next) ?? 1;
                case '*': Type = TokenType.Multiply; return Multiply(it.Next) ?? 1;
                case '/': Type = TokenType.Divide; return Divide(it.Next) ?? 1;
                case '%': Type = TokenType.Modulo; return Modulo(it.Next) ?? 1;
                case '(': Type = TokenType.LeftParenthesis; return 1;
                case ')': Type = TokenType.RightParenthesis; return 1;
                case '[': Type = TokenType.LeftBracket; return 1;
                case ']': Type = TokenType.RightBracket; return 1;
                case '{': Type = TokenType.LeftBrace; return 1;
                case '}': Type = TokenType.RightBrace; return 1;
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
            while (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match('0', '9') || c.Match("\\\x7F"))
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

        private int? NumberLiteral(LexicalIterator it)
        {
            char c = it.Current;
            while (c.Match('0', '9'))
            {
                it = it.Next;
                c = it.Current;
            }
            return it.Index;
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
                case ':': Type = TokenType.Separator; return 2;
                case '=': Type = TokenType.LeftAssign; return 2;
            }
            return null;
        }

        private int? Access(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '.': Type = TokenType.Range; return 2;
            }
            return null;
        }

        private int? Wild(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '!': Type = TokenType.ScriptComment; return LinerComment(it.Next);
            }
            return null;
        }

        private int? Annotation(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '@': Type = TokenType.Pragma; return 2;
            }
            return null;
        }

        private int? Conditional(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '?': Type = TokenType.Coalesce; return 2;
            }
            return null;
        }

        private int? Or(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '|': Type = TokenType.OrElse; return 2;
                case '=': Type = TokenType.OrLeftAssign; return 2;
            }
            return null;
        }

        private int? And(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '&': Type = TokenType.AndElse; return 2;
                case '=': Type = TokenType.AndLeftAssign; return 2;
            }
            return null;
        }

        private int? Xor(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.XorLeftAssign; return 2;
            }
            return null;
        }

        private int? Equal(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case ':': Type = TokenType.RightAssign; return 2;
                case '|': Type = TokenType.OrRightAssign; return 2;
                case '&': Type = TokenType.AndRightAssign; return 2;
                case '^': Type = TokenType.XorRightAssign; return 2;
                case '+': Type = TokenType.PlusRightAssign; return 2;
                case '-': Type = TokenType.MinusRightAssign; return 2;
                case '~': Type = TokenType.CombineRightAssign; return 2;
                case '*': Type = TokenType.MultiplyRightAssign; return 2;
                case '/': Type = TokenType.DivideRightAssign; return 2;
                case '%': Type = TokenType.ModuloRightAssign; return 2;
                case '<': Type = TokenType.LessThanOrEqual; return 2;
                case '>': Type = TokenType.GreaterThanOrEqual; return 2;
            }
            return null;
        }

        private int? LessThan(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.LessThanOrEqual; return 2;
                case '<': Type = TokenType.LeftShift; return 2;
                case '>': Type = TokenType.NotEqual; return 2;
            }
            return null;
        }

        private int? GreaterThan(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.GreaterThanOrEqual; return 2;
                case '<': Type = TokenType.NotEqual; return 2;
                case '>': Type = TokenType.RightShift; return 2;
            }
            return null;
        }

        private int? Plus(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.PlusLeftAssign; return 2;
                case '+': Type = TokenType.Increment; return 2;
            }
            return null;
        }

        private int? Minus(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.MinusLeftAssign; return 2;
                case '-': Type = TokenType.Decrement; return 2;
            }
            return null;
        }

        private int? Combine(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.CombineLeftAssign; return 2;
            }
            return null;
        }

        private int? Multiply(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.MultiplyLeftAssign; return 2;
            }
            return null;
        }

        private int? Divide(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.DivideLeftAssign; return 2;
                case '*': Type = TokenType.BlockComment; return BlockComment(it.Next);
                case '/': Type = TokenType.LineComment; return LinerComment(it.Next);
            }
            return null;
        }

        private int? Modulo(LexicalIterator it)
        {
            char c = it.Current;
            switch (c)
            {
                case '=': Type = TokenType.ModuloLeftAssign; return 2;
            }
            return null;
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