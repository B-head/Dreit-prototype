using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysisOld
{
    partial class Parser
    {
        private SyntaxOld SkipError(ref int c)
        {
            if(!IsEnable(c))
            {
                return null;
            }
            Token error = Peek(c++);
            List<SyntaxOld> child = new List<SyntaxOld>();
            child.Add(error);
            return CreateElement(child, TokenType.Error, c);
        }

        private SyntaxOld Spacer(ref int c)
        {
            List<SyntaxOld> child = new List<SyntaxOld>();
            bool error = false;
            while (IsEnable(c))
            {
                SyntaxOld s = CoalesceParser
                    (
                    ref c,
                    BlockComment,
                    LineComment
                    );
                if (s != null)
                {
                    child.Add(s);
                    continue;
                }
                Token t = Peek(c);
                if(t.Type == TokenType.LineTerminator || t.Type == TokenType.WhiteSpace)
                {
                    child.Add(t);
                    c++;
                    continue;
                }
                if (t.Type == TokenType.OtherString)
                {
                    child.Add(t);
                    c++;
                    error = true;
                    continue;
                }
                break;
            }
            return CreateElement(child, error ? TokenType.Error : TokenType.Spacer, c);
        }

        private SyntaxOld BlockComment(ref int c)
        {
            if(!IsEnable(c) || Peek(c).Type != TokenType.StartComment)
            {
                return null;
            }
            List<SyntaxOld> child = new List<SyntaxOld>();
            child.Add(Peek(c++));
            while (IsEnable(c))
            {
                SyntaxOld s = BlockComment(ref c);
                if(s != null)
                {
                    child.Add(s);
                    continue;
                }
                Token t = Peek(c);
                child.Add(t);
                c++;
                if (t.Type == TokenType.EndComment)
                {
                    break;
                }
            }
            return CreateElement(child, TokenType.BlockComment, c);
        }

        private SyntaxOld LineComment(ref int c)
        {
            if (!IsEnable(c) || Peek(c).Type != TokenType.StartLineComment)
            {
                return null;
            }
            List<SyntaxOld> child = new List<SyntaxOld>();
            child.Add(Peek(c++));
            while (IsEnable(c))
            {
                Token t = Peek(c);
                child.Add(t);
                c++;
                if (t.Type == TokenType.LineTerminator)
                {
                    break;
                }
            }
            return CreateElement(child, TokenType.LineComment, c);
        }
    }
}
