using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    abstract class Syntax
    {
        public SyntaxType Type { get; set; }
        public TextPosition Position { get; set; }
        public abstract string Text { get; set; }
        public abstract List<Syntax> Child { get; set; }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int indent)
        {
            return base.ToString();
        }

        protected string Indent(int indent)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }
    }

    class Token : Syntax
    {
        public override string Text { get; set; }
        public override List<Syntax> Child 
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
            return Indent(indent) + Position + ": " + Enum.GetName(typeof(SyntaxType), Type) + " => " + Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20') + "\n";
        }
    }

    class Element : Syntax
    {
        public override string Text
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override List<Syntax> Child { get; set; }

        public override string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Indent(indent) + Position + ": " + Enum.GetName(typeof(SyntaxType), Type));
            for (int i = 0; i < Child.Count; i++)
            {
                result.Append(Child[i].ToString(indent + 1));
            }
            return result.ToString();
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
        EndDirective,
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
    }
}
