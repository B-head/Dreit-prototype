using System;

namespace AbstractSyntax
{
    [Serializable]
    public struct Token
    {
        public TokenType Type { get; set; }
        public TextPosition Position { get; set; }
        public string Text { get; set; }

        public static readonly Token Empty = new Token { Text = string.Empty };

        public override string ToString()
        {
            return Position + ": " + Enum.GetName(typeof(TokenType), Type) + " => " + Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20');
        }

        public static implicit operator bool(Token token)
        {
            return !string.IsNullOrEmpty(token.Text);
        }
    }

    [Flags]
    public enum TokenType
    {
        Unknoun = 0x0,
        LineTerminator,
        WhiteSpace,
        BlockComment,
        LineCommnet,
        QuoteSeparator,
        PlainText,
        LetterStartString,
        DigitStartString,
        OtherString,

        EndExpression,
        Peir,
        Separator,
        List,
        Access,
        Range,
        Zone,
        Attribute,
        Pragma,
        Lambda,
        Conditional,
        Coalesce,
        Or,
        And,
        Not,
        Typeof,
        Refer,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Incompare,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        LeftCombine,
        RightCombine,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,
        LeftAssign = 0x10000,
        RightAssign = 0x20000,
    }
}
