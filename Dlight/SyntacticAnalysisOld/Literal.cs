using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysisOld
{
    partial class Parser
    {
        private SyntaxOld Identifier(ref int c)
        {
            return SequenceParser(SyntaxType.Identifier, ref c, SelectToken(SyntaxType.LetterStartString), Spacer);
        }

        private SyntaxOld PragmaLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.PragmaLiteral, ref c, null, SelectToken(SyntaxType.Pragma), ParentAccess);
        }

        private SyntaxOld IntegerLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.IntegerLiteral, ref c, null, SelectToken(SyntaxType.DigitStartString), Spacer);
        }

        private SyntaxOld RealLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.RealLiteral, ref c, IntegerLiteral, SelectToken(SyntaxType.Access), Spacer, IntegerLiteral);
        }

        private SyntaxOld StringLiteral(ref int c)
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
            List<SyntaxOld> child = new List<SyntaxOld>();
            child.Add(temp);
            while (IsEnable(c))
            {
                SyntaxOld s = BuiltIn(ref c);
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

        private SyntaxOld BuiltIn(ref int c)
        {
            return SequenceParser(SyntaxType.BuiltIn, ref c, null, SelectToken(SyntaxType.LeftBrace), Spacer, Expression, SelectToken(SyntaxType.RightBrace));
        }

        private SyntaxOld Argument(ref int c)
        {
            return SequenceParser(SyntaxType.Argument, ref c, ParentAccess, SelectToken(SyntaxType.Peir), Spacer, ParentAccess);
        }

        private SyntaxOld ArgumentList(ref int c)
        {
            return RepeatParser(SyntaxType.ArgumentList, ref c, Argument, SelectToken(SyntaxType.List), Spacer, Argument);
        }

        private SyntaxOld Attribute(ref int c)
        {
            return SequenceParser(SyntaxType.Attribute, ref c, null, CheckText(), Spacer);
        }

        private SyntaxOld Annotation(ref int c)
        {
            return SequenceParser(SyntaxType.Annotation, ref c, null, SelectToken(SyntaxType.At), Spacer, Identifier);
        }

        private SyntaxOld AttributeList(ref int c)
        {
            List<SyntaxOld> child = new List<SyntaxOld>();
            while (IsEnable(c))
            {
                SyntaxOld s = CoalesceParser
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

        private SyntaxOld Parameter(ref int c)
        {
            return SequenceParser(SyntaxType.Parameter, ref c, null, SelectToken(SyntaxType.LeftParenthesis), Spacer, ArgumentList, SelectToken(SyntaxType.RightParenthesis), Spacer);
        }

        private SyntaxOld CodeDefinition(ref int c)
        {
            return CoalesceParser
                (
                ref c,
                Hamper,
                Block
                );
        }

        private SyntaxOld Hamper(ref int c)
        {
            return SequenceParser(SyntaxType.Hamper, ref c, null, SelectToken(SyntaxType.Separator), Spacer, Directive);
        }

        private SyntaxOld Block(ref int c)
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
            List<SyntaxOld> child = new List<SyntaxOld>();
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
                SyntaxOld s = Directive(ref c);
                if (s != null)
                {
                    child.Add(s);
                    continue;
                }
            }
            child.Add(Spacer(ref c));
            return CreateElement(child, SyntaxType.Block, c);
        }

        private SyntaxOld EnumBlock(ref int c)
        {
            return SequenceParser(SyntaxType.EnumBlock, ref c, null, SelectToken(SyntaxType.LeftBrace), Spacer, EnumList, SelectToken(SyntaxType.RightBrace), Spacer);
        }

        private SyntaxOld EnumList(ref int c)
        {
            return RepeatParser(SyntaxType.EnumList, ref c, EnumPair, SelectToken(SyntaxType.List), Spacer, EnumPair);
        }

        private SyntaxOld EnumPair(ref int c)
        {
            return RepeatParser(SyntaxType.EnumPair, ref c, Identifier, SelectToken(SyntaxType.Peir), Spacer, Expression);
        }

        private SyntaxOld VariableLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.VariableLiteral, ref c, null, AttributeList, CheckText("var", "const"), Spacer, Argument);
        }

        private SyntaxOld RoutineLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.RoutineLiteral, ref c, null, AttributeList, CheckText("routine", "function"), Spacer, ParentAccess, Parameter, CodeDefinition);
        }

        private SyntaxOld LambdaLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.LambdaLiteral, ref c, null, SelectToken(SyntaxType.Lambda), Spacer, Parameter, CodeDefinition);
        }

        private SyntaxOld ClassLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.ClassLiteral, ref c, null, AttributeList, CheckText("class"), Spacer, ParentAccess, CodeDefinition);
        }

        private SyntaxOld EnumLiteral(ref int c)
        {
            return SequenceParser(SyntaxType.EnumLiteral, ref c, null, AttributeList, CheckText("enum"), Spacer, ParentAccess, EnumBlock);
        }
    }
}
