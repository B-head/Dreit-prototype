using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private Syntax SkipError(ref int c)
        {
            if(!IsEnable(c))
            {
                return null;
            }
            Token error = Peek(c++);
            List<Syntax> child = new List<Syntax>();
            child.Add(error);
            return CreateElement(child, SyntaxType.Error, c);
        }

        private Syntax Spacer(ref int c)
        {
            List<Syntax> child = new List<Syntax>();
            bool error = false;
            while (IsEnable(c))
            {
                Syntax s = CoalesceParser
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

        private Syntax BlockComment(ref int c)
        {
            if(!IsEnable(c) || Peek(c).Type != SyntaxType.StartComment)
            {
                return null;
            }
            List<Syntax> child = new List<Syntax>();
            child.Add(Peek(c++));
            while (IsEnable(c))
            {
                Syntax s = BlockComment(ref c);
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

        private Syntax LineComment(ref int c)
        {
            if (!IsEnable(c) || Peek(c).Type != SyntaxType.StartLineComment)
            {
                return null;
            }
            List<Syntax> child = new List<Syntax>();
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
