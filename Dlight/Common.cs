using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    static class Common
    {
        public static void Error(string message, string file, int line)
        {
            Console.WriteLine("Error " + file + "(" + line + "): " + message);
        }

        public static bool Match<V, T>(this V value, IEnumerable<T> list) where V : IEquatable<T>
        {
            foreach(T v in list)
            {
                if(value.Equals(v))
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

    struct Token
    {
        public string Value;
        public TokenType Type;
        public string File;
        public int Line;

        public Token(string value, TokenType type, string file, int line)
        {
            Value = value;
            Type = type;
            File = file;
            Line = line;
        }

        public override string ToString()
        {
            string temp = Value.Replace('\x0A', '\x20').Replace('\x0D', '\x20');
            return File + "(" + Line + "): " + Enum.GetName(typeof(TokenType), Type) + " => " + temp;
        }
    }

    enum TokenType
    {
        Unknoun,
        EndOfFile,
        EndOfLine,
        Space,
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
        BlockComment,
        LineComment,
        Escape,
        EndOfExpression,
        Pear,
        Separator,
        List,
        Access,
        Range,
        Wild,
        Annotation,
        Pragma,
        LeftAssign,
        RightAssign,
        Lambda,
        Conditional,
        Coalesce,
        OrElse,
        AndElse,
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
        Power,
        PowerLeftAssign,
        PowerRightAssign,
        Divide,
        DivideLeftAssign,
        DivideRightAssign,
        Modulo,
        ModuloLeftAssign,
        ModuloRightAssign,
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
