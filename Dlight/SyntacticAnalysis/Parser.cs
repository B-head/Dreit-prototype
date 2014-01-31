using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysis
{
    partial class Parser
    {
        private delegate Element ParserFunction(ref int c);
        private List<Token> InputToken;
        private List<Token> ErrorToken;

        public Element Parse(List<Token> input, string name)
        {
            InputToken = input;
            ErrorToken = new List<Token>();
            int c = 0;
            SkipSpaser(c);
            ExpressionList exp = Expression(ref c, true);
            return new Module { Name = name, ExpList = exp, ErrorToken = ErrorToken, Position = exp.Position };
        }

        private bool IsReadable(int c)
        {
            return c < InputToken.Count;
        }

        private Token Read(int c)
        {
            return IsReadable(c) ? InputToken[c] : null;
        }

        private void SkipSpaser(int c)
        {
            int temp = c;
            Spacer(ref temp);
            InputToken.RemoveRange(c, temp - c);
        }

        private void SkipError(int c)
        {
            SkipSpaser(c);
            AddError(c);
            InputToken.RemoveAt(c);
        }

        private void AddError(int c)
        {
            ErrorToken.Add(Read(c));
        }

        private bool CheckToken(int c, params TokenType[] type)
        {
            TokenType temp;
            return CheckToken(c, out temp, type);
        }

        private bool CheckToken(int c, out TokenType match, params TokenType[] type)
        {
            match = TokenType.Unknoun;
            Token t = Read(c);
            if(t == null)
            {
                return false;
            }
            foreach(TokenType v in type)
            {
                if(t.Type == v)
                {
                    match = v;
                    return true;
                }
            }
            return false;
        }

        private bool CheckText(int c, params string[] text)
        {
            if (!IsReadable(c))
            {
                return false;
            }
            Token t = Read(c);
            foreach (string v in text)
            {
                if (v == t.Text)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
