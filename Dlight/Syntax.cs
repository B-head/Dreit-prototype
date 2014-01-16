using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    struct TextPosition
    {
        public string File;
        public int Total;
        public int Line;
        public int Row;

        public TextPosition(string file, int total, int line, int row)
        {
            File = file;
            Total = total;
            Line = line;
            Row = row;
        }
    }

    abstract class Syntax
    {
        public SyntaxType Type { get; set; }
        public TextPosition Position { get; set; }

        public Syntax() { }

        public Syntax(SyntaxType type, string file, int line)
        {
            Type = type;
            Position = new TextPosition(file, int.MinValue, line, int.MinValue);
        }

        public override string ToString()
        {
            return ToValueString();
        }

        public abstract string ToValueString();
    }

    class Token : Syntax
    {
        public string Text;

        public Token() { }

        public Token(string value, SyntaxType type, string file, int line)
            : base(type, file, line)
        {
            Text = value;
        }

        public override string ToValueString()
        {
            return Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20');
        }

        public override string ToString()
        {
            return Position.File + "(" + Position.Line + "): " + Enum.GetName(typeof(SyntaxType), Type) + " => " + ToValueString();
        }
    }

    class Expression : Syntax
    {
        public List<Syntax> Child;

        public Expression() { }

        public Expression(List<Syntax> child, SyntaxType type, string file, int line)
            : base(type, file, line)
        {
            Child = child;
        }

        public override string ToValueString()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < Child.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(" ");
                }
                result.Append(Child[i].ToValueString());
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return Position.File + "(" + Position.Line + "): " + Enum.GetName(typeof(SyntaxType), Type) + " => " + ToValueString();
        }
    }

    enum SyntaxType
    {
        Unknoun,
        Root,

        EndOfFile,
        EndOfLine,
        WhiteSpace,
        LetterStartString,
        DigitStartString,
        OtherString,
        SingleQuote,
        DoubleQuote,
        BackQuote,
        StartComment,
        EndComment,
        LineComment,
        EndOfExpression,
        Pear,
        Separator,
        List,
        Access,
        Range,
        Wild,
        Annotation,
        Pragma,
        Lambda,
        Conditional,
        Coalesce,
        OrElse,
        AndElse,
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
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Incompare,
        LeftShift,
        LeftShiftLeftAssign,
        LeftShiftRightAssign,
        RightShift,
        RightShiftLeftAssign,
        RightShiftRightAssign,
        Plus,
        PlusLeftAssign,
        PlusRightAssign,
        Minus,
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
        Power,
        PowerLeftAssign,
        PowerRightAssign,
        Increment,
        Decrement,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,

        Identifier,
        BinaryPrefix,
        OctalPrefix,
        DecimalPrefix,
        HexPrefix,
        Number,
        RadixPoint,
        String,
        StartString,
        EndString,
        BuiltIn,
        StartBuiltIn,
        EndBuiltIn,
        Comment,
    }
}
