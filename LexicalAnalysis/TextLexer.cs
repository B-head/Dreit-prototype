using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LexicalAnalysis
{
    public partial class Lexer
    {
        private bool EndOfLine(ref TextPosition p)
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
            return TakeAddToken(ref p, i, TokenType.LineTerminator);
        }

        private bool WhiteSpace(ref TextPosition p)
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
            return TakeAddToken(ref p, i, TokenType.WhiteSpace);
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
            return TakeAddToken(ref p, i, TokenType.OtherString);
        }

        private bool StringLiteral(ref TextPosition p)
        {
            return false;
        }
    }
}
