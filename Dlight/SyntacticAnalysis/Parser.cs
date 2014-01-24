using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private delegate Syntax ParserFunction(ref int c);
        private List<Token> InputToken;
        private List<Token> ErrorToken;

        public ModuleElement Parse(List<Token> input, string name)
        {
            InputToken = input;
            ErrorToken = new List<Token>();
            int c = 0;
            SkipSpaser(c);
            List<Syntax> child = new List<Syntax>();
            while (IsReadable(c))
            {
                Syntax s = Expression(ref c);
                if(s == null)
                {
                    SkipError(c);
                    continue;
                }
                child.Add(s);
            }
            return new ModuleElement(name, child, ErrorToken);
        }

        public bool IsReadable(int c)
        {
            return c < InputToken.Count;
        }

        public Token Read(int c)
        {
            return IsReadable(c) ? InputToken[c] : null;
        }

        public void SkipSpaser(int c)
        {
            int temp = c;
            Spacer(ref temp);
            InputToken.RemoveRange(c, temp - c);
        }

        public void SkipError(int c)
        {
            SkipSpaser(c);
            AddError(c);
            InputToken.RemoveAt(c);
        }

        public bool CheckToken(int c, params SyntaxType[] type)
        {
            SyntaxType temp;
            return CheckToken(c, out temp, type);
        }

        public bool CheckToken(int c, out SyntaxType match, params SyntaxType[] type)
        {
            match = SyntaxType.Unknoun;
            Token t = Read(c);
            if(t == null)
            {
                return false;
            }
            foreach(SyntaxType v in type)
            {
                if(t.Type == v)
                {
                    match = v;
                    return true;
                }
            }
            return false;
        }

        public void AddError(int c)
        {
            ErrorToken.Add(Read(c));
        }
    }
}
