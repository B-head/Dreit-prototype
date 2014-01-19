using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Syntax Identifier(ref int c)
        {
            return SequenceParser(SyntaxType.Identifier, ref c, SelectToken(SyntaxType.LetterStartString), Spacer);
        }

        private Syntax PragmaLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.PragmaLiteral, ref c, null, SelectToken(SyntaxType.Pragma), ParentAccess);
        }

        private Syntax IntegerLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.IntegerLiteral, ref c, null, SelectToken(SyntaxType.DigitStartString), Spacer);
        }

        private Syntax RealLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.RealLiteral, ref c, IntegerLiteral, SelectToken(SyntaxType.Access), Spacer, IntegerLiteral);
        }

        private Syntax StringLiteral(ref int c)
        {
            if(!IsEnable(c))
            {
                return null;
            }
            Token temp = Peek(c);
            SyntaxType sep = temp.Type;
            if (sep != SyntaxType.SingleQuote && sep != SyntaxType.DoubleQuote && sep != SyntaxType.BackQuote)
            {
                return null;
            }
            c++;
            List<Syntax> child = new List<Syntax>();
            child.Add(temp);
            while (IsEnable(c))
            {
                Syntax s = BuiltIn(ref c);
                if (s != null)
                {
                    child.Add(s);
                    continue;
                }
                Token t = Peek(c);
                child.Add(t);
                c++;
                if (t.Type == sep)
                {
                    break;
                }
            }
            child.Add(Spacer(ref c));
            return CreateElement(child, SyntaxType.StringLiteral, c);
        }

        private Syntax BuiltIn(ref int c)
        {
            return SequenceParser(SyntaxType.BuiltIn, ref c, null, SelectToken(SyntaxType.LeftBrace), Spacer, Expression, SelectToken(SyntaxType.RightBrace));
        }

        private Syntax Argument(ref int c)
        {
            return SequenceParser(SyntaxType.Argument, ref c, ParentAccess, SelectToken(SyntaxType.Peir), Spacer, ParentAccess);
        }

        private Syntax ArgumentList(ref int c)
        {
            return RepeatParser(SyntaxType.ArgumentList, ref c, Argument, SelectToken(SyntaxType.List), Spacer, Argument);
        }

        private Syntax Attribute(ref int c)
        {
            return SequenceParser(SyntaxType.Attribute, ref c, null, CheckText(), Spacer);
        }

        private Syntax Annotation(ref int c)
        {
            return SequenceParser(SyntaxType.Annotation, ref c, null, SelectToken(SyntaxType.At), Spacer, Identifier);
        }

        private Syntax AttributeList(ref int c)
        {
            List<Syntax> child = new List<Syntax>();
            while (IsEnable(c))
            {
                Syntax s = CoalesceParser
                    (
                    ref c,
                    Attribute,
                    Annotation
                    );
                if (s == null)
                {
                    break;
                }
                child.Add(s);
            }
            return CreateElement(child, SyntaxType.AttributeList, c);
        }

        private Syntax Parameter(ref int c)
        {
            return SequenceParser(SyntaxType.Parameter, ref c, null, SelectToken(SyntaxType.LeftParenthesis), Spacer, ArgumentList, SelectToken(SyntaxType.RightParenthesis), Spacer);
        }

        private Syntax CodeDefinition(ref int c)
        {
            return CoalesceParser
                (
                ref c,
                Hamper,
                Block
                );
        }

        private Syntax Hamper(ref int c)
        {
            return SequenceParser(SyntaxType.Hamper, ref c, null, SelectToken(SyntaxType.Separator), Spacer, Directive);
        }

        private Syntax Block(ref int c)
        {
            if (!IsEnable(c))
            {
                return null;
            }
            Token temp = Peek(c);
            if (temp.Type != SyntaxType.LeftBrace)
            {
                return null;
            }
            c++;
            List<Syntax> child = new List<Syntax>();
            child.Add(temp);
            while (IsEnable(c))
            {
                Token t = Peek(c);
                if (t.Type == SyntaxType.RightBrace)
                {
                    child.Add(t);
                    c++;
                    break;
                }
                Syntax s = Directive(ref c);
                if (s != null)
                {
                    child.Add(s);
                    continue;
                }
            }
            child.Add(Spacer(ref c));
            return CreateElement(child, SyntaxType.Block, c);
        }

        private Syntax EnumBlock(ref int c)
        {
            return SequenceParser(SyntaxType.EnumBlock, ref c, null, SelectToken(SyntaxType.LeftBrace), Spacer, EnumList, SelectToken(SyntaxType.RightBrace), Spacer);
        }

        private Syntax EnumList(ref int c)
        {
            return RepeatParser(SyntaxType.EnumList, ref c, EnumPair, SelectToken(SyntaxType.List), Spacer, EnumPair);
        }

        private Syntax EnumPair(ref int c)
        {
            return RepeatParser(SyntaxType.EnumPair, ref c, Identifier, SelectToken(SyntaxType.Peir), Spacer, Expression);
        }

        private Syntax VariableLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.VariableLiteral, ref c, null, AttributeList, CheckText("var", "const"), Spacer, Argument);
        }

        private Syntax RoutineLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.RoutineLiteral, ref c, null, AttributeList, CheckText("routine", "function"), Spacer, ParentAccess, Parameter, CodeDefinition);
        }

        private Syntax LambdaLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.LambdaLiteral, ref c, null, SelectToken(SyntaxType.Lambda), Spacer, Parameter, CodeDefinition);
        }

        private Syntax ClassLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.ClassLiteral, ref c, null, AttributeList, CheckText("class"), Spacer, ParentAccess, CodeDefinition);
        }

        private Syntax EnumLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.EnumLiteral, ref c, null, AttributeList, CheckText("enum"), Spacer, ParentAccess, EnumBlock);
        }
    }
}
