using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private List<Token> List;

        private delegate Syntax ParserFunction(ref int c);

        public Syntax Parse(List<Token> list)
        {
            List = list;
            int c = 0;
            List<Syntax> child = new List<Syntax>();
            while(IsEnable(c))
            {
                Syntax s = Directive(ref c);
                child.Add(s);
            }
            return CreateElement(child, SyntaxType.Root, c);
        }

        private bool IsEnable(int c)
        {
            return c < List.Count;
        }

        private Token Peek(int c)
        {
            return List[c];
        }

        private Syntax CreateElement(List<Syntax> child, SyntaxType type, int c)
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
            return new Element { Child = child, Type = type, Position = position };
        }

        private Syntax CoalesceParser(ref int c, params ParserFunction[] func)
        {
            Syntax result = null;
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

        private Syntax SequenceParser(SyntaxType type, ref int c, ParserFunction firstFunc, params ParserFunction[] func)
        {
            int temp = c;
            List<Syntax> child = new List<Syntax>();
            Syntax first = null;
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
                Syntax s = f(ref temp);
                if (s == null)
                {
                    return first;
                }
                child.Add(s);
            }
            c = temp;
            return CreateElement(child, type, c);
        }

        private Syntax RepeatParser(SyntaxType type, ref int c, ParserFunction firstFunc, params ParserFunction[] func)
        {
            int temp = c;
            Syntax first = firstFunc(ref temp);
            if (first == null)
            {
                return null;
            }
            c = temp;
            List<Syntax> child = new List<Syntax>();
            List<Syntax> add = new List<Syntax>();
            child.Add(first);
            while (true)
            {
                add.Clear();
                foreach (ParserFunction f in func)
                {
                    Syntax s = f(ref temp);
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

        private ParserFunction SelectToken(params SyntaxType[] type)
        {
            return (ref int c) =>
            {
                if(!IsEnable(c))
                {
                    return null;
                }
                Token t = Peek(c);
                foreach (SyntaxType v in type)
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
