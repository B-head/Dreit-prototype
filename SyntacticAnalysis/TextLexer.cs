using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace SyntacticAnalysis
{
    public partial class Lexer
    {
        private Token EndOfLine(Tokenizer t)
        {
            int i = 0;
            if (t.IsReadable(i) && t.MatchAny(i, "\x0A"))
            {
                i++;
                if (t.IsReadable(i) && t.MatchAny(i, "\x0D"))
                {
                    i++;
                }
            }
            else if (t.IsReadable(i) && t.MatchAny(i, "\x0D"))
            {
                i++;
                if (t.IsReadable(i) && t.MatchAny(i, "\x0A"))
                {
                    i++;
                }
            }
            return t.TakeToken(i, TokenType.LineTerminator);
        }

        private Token WhiteSpace(Tokenizer t)
        {
            int i;
            for (i = 0; t.IsReadable(i); i++)
            {
                if (t.MatchRange(i, '\x00', '\x20') || t.MatchAny(i, "\x7F"))
                {
                    continue;
                }
                break;
            }
            return t.TakeToken(i, TokenType.WhiteSpace);
        }

        private bool BlockComment(ref TextPosition p)
        {
            int i = 0, nest = 1;
            if(!(IsEnable(p, i + 1) && Peek(p, i).Match("/") && Peek(p, i + 1).Match("*")))
            {
                return false;
            }
            for (i = 2; IsEnable(p, i); i++)
            {
                if(IsEnable(p, i + 1) && Peek(p, i).Match("*") && Peek(p, i + 1).Match("/"))
                {
                    ++i;
                    if (--nest == 0)
                    {
                        ++i;
                        break;
                    }
                }
                if(IsEnable(p, i + 1) && Peek(p, i).Match("/") && Peek(p, i + 1).Match("*"))
                {
                    ++i;
                    ++nest;
                }
            }
            SkipToken(ref p, i);
            return true;
        }

        private bool LineCommnet(ref TextPosition p)
        {
            int i = 0;
            if (!(IsEnable(p, i + 1) && Peek(p, i).Match("/") && Peek(p, i + 1).Match("/")))
            {
                if (!(IsEnable(p, i + 1) && Peek(p, i).Match("#") && Peek(p, i + 1).Match("!")))
                {
                    return false;
                }
            }
            for (i = 2; IsEnable(p, i); i++)
            {
                if (IsEnable(p, i) && Peek(p, i).Match("\x0A\x0D"))
                {
                    break;
                }
            }
            SkipToken(ref p, i);
            return true;
        }

        private bool StringLiteral(ref TextPosition p)
        {
            if (!(IsEnable(p, 0) && Peek(p, 0).Match("\'\"`")))
            {
                return false;
            }
            string quote = Peek(p, 0).ToString();
            TakeAddToken(ref p, 1, TokenType.QuoteSeparator);
            int i;
            for (i = 0; IsEnable(p, i); i++)
            {
                char c = Peek(p, i);
                if (c.Match(quote))
                {
                    TakeAddToken(ref p, i, TokenType.PlainText);
                    return TakeAddToken(ref p, 1, TokenType.QuoteSeparator);
                }
                if (c.Match("{"))
                {
                    TakeAddToken(ref p, i, TokenType.PlainText);
                    BuiltInExpression(ref p);
                    i = -1;
                }
            }
            return TakeAddToken(ref p, i, TokenType.PlainText);
        }

        private bool BuiltInExpression(ref TextPosition p)
        {
            if (!(IsEnable(p, 0) && Peek(p, 0).Match("{")))
            {
                return false;
            }
            int nest = 0;
            while (IsEnable(p, 0))
            {
                DisjunctionLexer
                    (
                    ref p,
                    EndOfLine,
                    WhiteSpace,
                    BlockComment,
                    LineCommnet,
                    StringLiteral,
                    LetterStartString,
                    DigitStartString,
                    TriplePunctuator,
                    DoublePunctuator,
                    SinglePunctuator,
                    OtherString
                    );
                if (_Token[_Token.Count - 1].Type == TokenType.LeftBrace)
                {
                    ++nest;
                }
                if(_Token[_Token.Count - 1].Type == TokenType.RightBrace)
                {
                    if(--nest == 0)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        private bool LetterStartString(ref TextPosition p)
        {
            int i;
            bool escape = false;
            for (i = 0; IsEnable(p, i); i++)
            {
                char c = Peek(p, i);
                if (escape && c.Match('!', '~'))
                {
                    escape = false;
                    continue;
                }
                if (c.Match('a', 'z') || c.Match('A', 'Z') || c.Match("_"))
                {
                    continue;
                }
                if (i > 0 && c.Match('0', '9'))
                {
                    continue;
                }
                if (c.Match("\\"))
                {
                    escape = true;
                    continue;
                }
                break;
            }
            return TakeAddToken(ref p, i, TokenType.LetterStartString);
        }

        private bool DigitStartString(ref TextPosition p)
        {
            int i;
            bool escape = false;
            for (i = 0; IsEnable(p, i); i++)
            {
                char c = Peek(p, i);
                if (escape && c.Match('!', '~'))
                {
                    escape = false;
                    continue;
                }
                if (c.Match('0', '9') || c.Match("_"))
                {
                    continue;
                }
                if (i > 0 && (c.Match('a', 'z') || c.Match('A', 'Z')))
                {
                    continue;
                }
                if (i > 0 && c.Match("\\"))
                {
                    escape = true;
                    continue;
                }
                break;
            }
            return TakeAddToken(ref p, i, TokenType.DigitStartString);
        }

        private bool OtherString(ref TextPosition p)
        {
            int i;
            for (i = 0; IsEnable(p, i); i++)
            {
                char c = Peek(p, i);
                if (!c.Match('\x00', '\x7F'))
                {
                    continue;
                }
                break;
            }
            return TakeAddErrorToken(ref p, i, TokenType.OtherString);
        }
    }
}
