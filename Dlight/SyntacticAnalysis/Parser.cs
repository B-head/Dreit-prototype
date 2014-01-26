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

        private Syntax LeftJoinBinomial(ref int c, ParserFunction next, params TokenType[] type)
        {
            Syntax left = next(ref c);
            if (left == null)
            {
                return null;
            }
            while (IsReadable(c))
            {
                TokenType match;
                if (!CheckToken(c, out match, type))
                {
                    break;
                }
                SkipSpaser(++c);
                Syntax right = next(ref c);
                left = new Binomial { Left = left, Right = right, Operation = match, Position = left.Position };
            }
            return left;
        }

        private Syntax RepeatSet<SET>(ref int c, ParserFunction next, params TokenType[] type) where SET : ExpressionSet, new()
        {
            List<Syntax> child = new List<Syntax>();
            List<TokenType> expType = new List<TokenType>();
            Syntax first = next(ref c);
            if (first == null)
            {
                return null;
            }
            child.Add(first);
            while (IsReadable(c))
            {
                TokenType match;
                if (!CheckToken(c, out match, type))
                {
                    break;
                }
                expType.Add(match);
                SkipSpaser(++c);
                child.Add(next(ref c));
            }
            if(child.Count <= 1)
            {
                return first;
            }
            return new SET { Child = child, ExpType = expType, Position = first.Position };
        }
    }
}
