using AbstractSyntax;

namespace SyntacticAnalysis
{
    class Tokenizer
    {
        public string Text { get; private set; }
        public TextPosition Position { get; private set; }

        public Tokenizer(string text, string fileName)
        {
            Text = text;
            Position = new TextPosition { File = fileName, Line = 1 };
        }

        public bool IsReadable(int index = 0)
        {
            return Position.Total + index < Text.Length;
        }

        public char Read(int index = 0)
        {
            return Text[Position.Total + index];
        }

        public string Read(int index, int length)
        {
            int start = Position.Total + index;
            if (start + length <= Text.Length)
            {
                return Text.Substring(start, length);
            }
            else
            {
                return string.Empty;
            }
        }

        public bool MatchAny(int index, string list)
        {
            var c = Read(index);
            foreach (var v in list)
            {
                if (v == c)
                {
                    return true;
                }
            }
            return false;
        }

        public bool MatchRange(int index, char start, char end)
        {
            var c = Read(index);
            return start <= c && c <= end;
        }

        public Token TakeToken(int length, TokenType type)
        {
            if (length == 0)
            {
                return null;
            }
            string text = Read(0, length);
            var temp = Position;
            temp.Length = length;
            Token token = new Token { Text = text, Type = type, Position = temp };
            var p = Position;
            p.Total += length;
            p.Row += length;
            if (type == TokenType.LineTerminator)
            {
                p.Line++;
                p.Row = 0;
            }
            Position = p;
            return token;
        }
    }
}
