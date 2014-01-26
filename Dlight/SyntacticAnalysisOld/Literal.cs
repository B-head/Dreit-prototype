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
            return SequenceParser(TokenType.Identifier, ref c, SelectToken(TokenType.LetterStartString), Spacer);
        }

        private SyntaxOld PragmaLiteral(ref int c)
        {
            return SequenceParser(TokenType.PragmaLiteral, ref c, null, SelectToken(TokenType.Pragma), ParentAccess);
        }

        private SyntaxOld IntegerLiteral(ref int c)
        {
            return SequenceParser(TokenType.IntegerLiteral, ref c, null, SelectToken(TokenType.DigitStartString), Spacer);
        }

        private SyntaxOld RealLiteral(ref int c)
        {
            return SequenceParser(TokenType.RealLiteral, ref c, IntegerLiteral, SelectToken(TokenType.Access), Spacer, IntegerLiteral);
        }

        private SyntaxOld StringLiteral(ref int c)
        {
            if(!IsEnable(c))
            {
                return null;
            }
            Token temp = Peek(c);
            TokenType sep = temp.Type;
            if (sep != TokenType.SingleQuote && sep != TokenType.DoubleQuote && sep != TokenType.BackQuote)
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
            return CreateElement(child, TokenType.StringLiteral, c);
        }

        private SyntaxOld BuiltIn(ref int c)
        {
            return SequenceParser(TokenType.BuiltIn, ref c, null, SelectToken(TokenType.LeftBrace), Spacer, Expression, SelectToken(TokenType.RightBrace));
        }

        private SyntaxOld Argument(ref int c)
        {
            return SequenceParser(TokenType.Argument, ref c, ParentAccess, SelectToken(TokenType.Peir), Spacer, ParentAccess);
        }

        private SyntaxOld ArgumentList(ref int c)
        {
            return RepeatParser(TokenType.ArgumentList, ref c, Argument, SelectToken(TokenType.List), Spacer, Argument);
        }

        private SyntaxOld Attribute(ref int c)
        {
            return SequenceParser(TokenType.Attribute, ref c, null, CheckText(), Spacer);
        }

        private SyntaxOld Annotation(ref int c)
        {
            return SequenceParser(TokenType.Annotation, ref c, null, SelectToken(TokenType.At), Spacer, Identifier);
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
            return CreateElement(child, TokenType.AttributeList, c);
        }

        private SyntaxOld Parameter(ref int c)
        {
            return SequenceParser(TokenType.Parameter, ref c, null, SelectToken(TokenType.LeftParenthesis), Spacer, ArgumentList, SelectToken(TokenType.RightParenthesis), Spacer);
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
            return SequenceParser(TokenType.Hamper, ref c, null, SelectToken(TokenType.Separator), Spacer, Directive);
        }

        private SyntaxOld Block(ref int c)
        {
            if (!IsEnable(c))
            {
                return null;
            }
            Token temp = Peek(c);
            if (temp.Type != TokenType.LeftBrace)
            {
                return null;
            }
            c++;
            List<SyntaxOld> child = new List<SyntaxOld>();
            child.Add(temp);
            while (IsEnable(c))
            {
                Token t = Peek(c);
                if (t.Type == TokenType.RightBrace)
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
            return CreateElement(child, TokenType.Block, c);
        }

        private SyntaxOld EnumBlock(ref int c)
        {
            return SequenceParser(TokenType.EnumBlock, ref c, null, SelectToken(TokenType.LeftBrace), Spacer, EnumList, SelectToken(TokenType.RightBrace), Spacer);
        }

        private SyntaxOld EnumList(ref int c)
        {
            return RepeatParser(TokenType.EnumList, ref c, EnumPair, SelectToken(TokenType.List), Spacer, EnumPair);
        }

        private SyntaxOld EnumPair(ref int c)
        {
            return RepeatParser(TokenType.EnumPair, ref c, Identifier, SelectToken(TokenType.Peir), Spacer, Expression);
        }

        private SyntaxOld VariableLiteral(ref int c)
        {
            return SequenceParser(TokenType.VariableLiteral, ref c, null, AttributeList, CheckText("var", "const"), Spacer, Argument);
        }

        private SyntaxOld RoutineLiteral(ref int c)
        {
            return SequenceParser(TokenType.RoutineLiteral, ref c, null, AttributeList, CheckText("routine", "function"), Spacer, ParentAccess, Parameter, CodeDefinition);
        }

        private SyntaxOld LambdaLiteral(ref int c)
        {
            return SequenceParser(TokenType.LambdaLiteral, ref c, null, SelectToken(TokenType.Lambda), Spacer, Parameter, CodeDefinition);
        }

        private SyntaxOld ClassLiteral(ref int c)
        {
            return SequenceParser(TokenType.ClassLiteral, ref c, null, AttributeList, CheckText("class"), Spacer, ParentAccess, CodeDefinition);
        }

        private SyntaxOld EnumLiteral(ref int c)
        {
            return SequenceParser(TokenType.EnumLiteral, ref c, null, AttributeList, CheckText("enum"), Spacer, ParentAccess, EnumBlock);
        }
    }
}
