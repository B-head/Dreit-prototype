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
            return CreateElement(child, SyntaxType.Error, c);
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
                if(t.Type == SyntaxType.EndLine || t.Type == SyntaxType.WhiteSpace)
                {
                    child.Add(t);
                    c++;
                    continue;
                }
                if (t.Type == SyntaxType.OtherString)
                {
                    child.Add(t);
                    c++;
                    error = true;
                    continue;
                }
                break;
            }
            return CreateElement(child, error ? SyntaxType.Error : SyntaxType.Spacer, c);
        }

        private SyntaxOld BlockComment(ref int c)
        {
            if(!IsEnable(c) || Peek(c).Type != SyntaxType.StartComment)
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
                if (t.Type == SyntaxType.EndComment)
                {
                    break;
                }
            }
            return CreateElement(child, SyntaxType.BlockComment, c);
        }

        private SyntaxOld LineComment(ref int c)
        {
            if (!IsEnable(c) || Peek(c).Type != SyntaxType.StartLineComment)
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
                if (t.Type == SyntaxType.EndLine)
                {
                    break;
                }
            }
            return CreateElement(child, SyntaxType.LineComment, c);
        }
    }
}
