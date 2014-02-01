using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class Token : SyntaxOld
    {
        public override string Text { get; set; }
        public override List<SyntaxOld> Child
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override string ToString(int indent)
        {
            return Position + ": " + Enum.GetName(typeof(TokenType), Type) + " => " + Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20') + "\n";
        }
    }

    struct TextPosition
    {
        public string File;
        public int Total;
        public int Line;
        public int Row;
        public int Length;

        public TextPosition(string file, int total, int line, int row)
        {
            File = file;
            Total = total;
            Line = line;
            Row = row;
            Length = 0;
        }

        public override string ToString()
        {
            return File + "(" + Line + "," + Row + ")";
        }
    }

    enum TokenType
    {
        Unknoun,
        Special,

        Root,
        Spacer,
        Error,
        BlockComment,
        LineComment,
        Import,
        Using,
        Alias,
        WildAttribute,
        Assign,
        Tuple,
        PeirLiteral,
        RangeLiteral,
        Logical,
        Compare,
        Bitwise,
        Shift,
        Addtive,
        Multiplicative,
        Powertive,
        Unary,
        GorupExpression,
        Identifier,
        PragmaLiteral,
        MenberAccess,
        ParentAccess,
        IntegerLiteral,
        RealLiteral,
        StringLiteral,
        BuiltIn,
        Argument,
        ArgumentList,
        Parameter,
        Attribute,
        Annotation,
        AttributeList,
        Hamper,
        Block,
        EnumBlock,
        EnumList,
        EnumPair,
        VariableLiteral,
        RoutineLiteral,
        LambdaLiteral,
        ClassLiteral,
        EnumLiteral,

        LineTerminator,
        WhiteSpace,
        LetterStartString,
        DigitStartString,
        OtherString,
        SingleQuote,
        DoubleQuote,
        BackQuote,
        StartComment,
        EndComment,
        StartLineComment,
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
