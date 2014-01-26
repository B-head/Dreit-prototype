using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysisOld
{
    partial class Parser
    {
        private delegate SyntaxOld ParserFunction(ref int c);
        private List<Token> List;

        public SyntaxOld Parse(List<Token> list)
        {
            List = list;
            int c = 0;
            List<SyntaxOld> child = new List<SyntaxOld>();
            while(IsEnable(c))
            {
                SyntaxOld s = Directive(ref c);
                child.Add(s);
            }
            return CreateElement(child, TokenType.Root, c);
        }

        private bool IsEnable(int c)
        {
            return c < List.Count;
        }

        private Token Peek(int c)
        {
            return List[c];
        }

        private SyntaxOld CreateElement(List<SyntaxOld> child, TokenType type, int c)
        {
            TextPosition position = new TextPosition();
            if(child.Count > 0)
            {
                position = child[0].Position;
            }
            else if(IsEnable(c))
            {
                position = Peek(c).Position;
            }
            return new DirectiveOld { Child = child, Type = type, Position = position };
        }

        private SyntaxOld CoalesceParser(ref int c, params ParserFunction[] func)
        {
            SyntaxOld result = null;
            foreach (ParserFunction f in func)
            {
                int temp = c;
                result = f(ref temp);
                if(result != null)
                {
                    c = temp;
                    break;
                }
            }
            return result;
        }

        private SyntaxOld SequenceParser(TokenType type, ref int c, ParserFunction firstFunc, params ParserFunction[] func)
        {
            int temp = c;
            List<SyntaxOld> child = new List<SyntaxOld>();
            SyntaxOld first = null;
            if (firstFunc != null)
            {
                first = firstFunc(ref temp);
                if (first == null)
                {
                    return null;
                }
                c = temp;
                child.Add(first);
            }
            foreach (ParserFunction f in func)
            {
                SyntaxOld s = f(ref temp);
                if (s == null)
                {
                    return first;
                }
                child.Add(s);
            }
            c = temp;
            return CreateElement(child, type, c);
        }

        private SyntaxOld RepeatParser(TokenType type, ref int c, ParserFunction firstFunc, params ParserFunction[] func)
        {
            int temp = c;
            SyntaxOld first = firstFunc(ref temp);
            if (first == null)
            {
                return null;
            }
            c = temp;
            List<SyntaxOld> child = new List<SyntaxOld>();
            List<SyntaxOld> add = new List<SyntaxOld>();
            child.Add(first);
            while (true)
            {
                add.Clear();
                foreach (ParserFunction f in func)
                {
                    SyntaxOld s = f(ref temp);
                    if (s == null)
                    {
                        goto end;
                    }
                    child.Add(s);
                }
                c = temp;
                child.AddRange(add);
            }
            end:
            if (child.Count > 1)
            {
                return CreateElement(child, type, c);
            }
            else
            {
                return first;
            }
        }

        private ParserFunction SelectToken(params TokenType[] type)
        {
            return (ref int c) =>
            {
                if(!IsEnable(c))
                {
                    return null;
                }
                Token t = Peek(c);
                foreach (TokenType v in type)
                {
                    if(v == t.Type)
                    {
                        c++;
                        return t;
                    }
                }
                return null;
            };
        }

        private ParserFunction CheckText(params string[] text)
        {
            return (ref int c) =>
            {
                if (!IsEnable(c))
                {
                    return null;
                }
                Token t = Peek(c);
                foreach (string v in text)
                {
                    if (v == t.Text)
                    {
                        c++;
                        return t;
                    }
                }
                return null;
            };
        }
    }
}
