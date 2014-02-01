using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private void Spacer(ref int c)
        {
            while (IsReadable(c))
            {
                BlockComment(ref c);
                LineComment(ref c);
                if (CheckToken(c, TokenType.WhiteSpace, TokenType.LineTerminator))
                {
                    c++;
                    continue;
                }
                if (CheckToken(c, TokenType.OtherString))
                {
                    c++;
                    AddError(c);
                    continue;
                }
                break;
            }
        }

        private void BlockComment(ref int c)
        {
            Token t = Read(c);
            if (!CheckToken(c, TokenType.StartComment))
            {
                return;
            }
            c++;
            while (IsReadable(c))
            {
                BlockComment(ref c);
                if (CheckToken(c++, TokenType.EndComment))
                {
                    break;
                }
            }
        }

        private void LineComment(ref int c)
        {
            Token t = Read(c);
            if (!CheckToken(c, TokenType.StartLineComment))
            {
                return;
            }
            c++;
            while (IsReadable(c))
            {
                if (CheckToken(c++, TokenType.LineTerminator))
                {
                    break;
                }
            }
        }
    }
}
