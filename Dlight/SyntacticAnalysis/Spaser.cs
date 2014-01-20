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
                if (CheckToken(c, SyntaxType.WhiteSpace, SyntaxType.EndLine))
                {
                    c++;
                    continue;
                }
                if (CheckToken(c, SyntaxType.OtherString))
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
            if (!CheckToken(c, SyntaxType.StartComment))
            {
                return;
            }
            c++;
            while (IsReadable(c))
            {
                BlockComment(ref c);
                if (CheckToken(c++, SyntaxType.EndComment))
                {
                    break;
                }
            }
        }

        private void LineComment(ref int c)
        {
            Token t = Read(c);
            if (!CheckToken(c, SyntaxType.StartLineComment))
            {
                return;
            }
            c++;
            while (IsReadable(c))
            {
                if (CheckToken(c++, SyntaxType.EndLine))
                {
                    break;
                }
            }
        }
    }
}
