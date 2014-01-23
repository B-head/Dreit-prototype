using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    static class Common
    {
        public static string Indent(int indent)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }

        public static bool Match<V, T>(this V value, IEnumerable<T> list) where V : IEquatable<T>
        {
            foreach (T v in list)
            {
                if (value.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Match<V>(this V value, V stert, V end) where V : IComparable<V>
        {
            return value.CompareTo(stert) >= 0 && value.CompareTo(end) <= 0;
        }
    }

    interface Translator
    {
        void Save();
        Translator CreateModule(Scope<Element> scope);
        void GenelateNumber(int value);
        void GenelateBinomial(Type dataType, SyntaxType operation);
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
            return File + "(" + Line + ")";
        }
    }

    enum SyntaxType
    {
        Unknoun,
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

        EndLine,
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
