using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.LexicalAnalysis
{
    partial class Lexer
    {
        private Token EndOfLine(ref TextPosition p)
        {
            int i = 0;
            if (IsEnable(p, i) && Peek(p, i).Match("\x0A"))
            {
                i++;
                if (IsEnable(p, i) && Peek(p, i).Match("\x0D"))
                {
                    i++;
                }
            }
            else if (IsEnable(p, i) && Peek(p, i).Match("\x0D"))
            {
                i++;
                if (IsEnable(p, i) && Peek(p, i).Match("\x0A"))
                {
                    i++;
                }
            }
            return TakeToken(ref p, i, TokenType.EndLine);
        }

        private Token WhiteSpace(ref TextPosition p)
        {
            int i;
            for (i = 0; IsEnable(p, i); i++)
            {
                char c = Peek(p, i);
                if (c.Match('\x00', '\x20') || c.Match("\x7F"))
                {
                    continue;
                }
                break;
            }
            return TakeToken(ref p, i, TokenType.WhiteSpace);
        }

        private Token LetterStartString(ref TextPosition p)
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
            return TakeToken(ref p, i, TokenType.LetterStartString);
        }

        private Token DigitStartString(ref TextPosition p)
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
            return TakeToken(ref p, i, TokenType.DigitStartString);
        }

        private Token OtherString(ref TextPosition p)
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
            return TakeToken(ref p, i, TokenType.OtherString);
        }
    }
}
