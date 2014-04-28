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

    public enum TokenType
    {
        Unknoun,
        Special,
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
        Wild,
        At,
        Pragma,
        Lambda,
        Conditional,
        Coalesce,
        OrElse,
        AndElse,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Incompare,
        LeftAssign,
        RightAssign,
        Or,
        OrLeftAssign,
        OrRightAssign,
        And,
        AndLeftAssign,
        AndRightAssign,
        Xor,
        XorLeftAssign,
        XorRightAssign,
        Not,
        LeftShift,
        LeftShiftLeftAssign,
        LeftShiftRightAssign,
        RightShift,
        RightShiftLeftAssign,
        RightShiftRightAssign,
        ArithRightShift,
        ArithRightShiftLeftAssign,
        ArithRightShiftRightAssign,
        LeftRotate,
        LeftRotateLeftAssign,
        LeftRotateRightAssign,
        RightRotate,
        RightRotateLeftAssign,
        RightRotateRightAssign,
        Add,
        PlusLeftAssign,
        PlusRightAssign,
        Subtract,
        MinusLeftAssign,
        MinusRightAssign,
        Combine,
        CombineLeftAssign,
        CombineRightAssign,
        Multiply,
        MultiplyLeftAssign,
        MultiplyRightAssign,
        Divide,
        DivideLeftAssign,
        DivideRightAssign,
        Modulo,
        ModuloLeftAssign,
        ModuloRightAssign,
        Exponent,
        ExponentLeftAssign,
        ExponentRightAssign,
        Increment,
        Decrement,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,
    }
}
