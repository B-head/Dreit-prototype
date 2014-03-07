using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using Common;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private delegate Element ParserFunction(ref int c);
        private delegate E ParserFunction<E>(ref int c) where E : Element;
        private List<Token> InputToken;
        private List<Token> ErrorToken;

        public Element Parse(List<Token> input, string name)
        {
            InputToken = input;
            ErrorToken = new List<Token>();
            int c = 0;
            SkipSpaser(c);
            DirectiveList exp = DirectiveList(ref c, true);
            return new DeclateModule { Name = name, ExpList = exp, ErrorToken = ErrorToken, Position = exp.Position };
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

        private Element CoalesceParser(ref int c, params ParserFunction[] func)
        {
            Element result = null;
            foreach (ParserFunction f in func)
            {
                int temp = c;
                result = f(ref temp);
                if (result != null)
                {
                    c = temp;
                    break;
                }
            }
            return result;
        }

        private Element LeftAssociative<R>(ref int c, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(ref c);
            TokenType match;
            while (CheckToken(c, out match, type))
            {
                SkipSpaser(++c);
                Element right = next(ref c);
                left = new R { Left = left, Right = right, Operator = match, Position = left.Position };
            }
            return left;
        }

        private Element RightAssociative<R>(ref int c, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(ref c);
            TokenType match;
            if (!CheckToken(c, out match, type))
            {
                return left;
            }
            SkipSpaser(++c);
            Element right = RightAssociative<R>(ref c, next, type);
            return new R { Left = left, Right = right, Operator = match, Position = left.Position };
        }

        private TupleList ParseTuple(ref int c, ParserFunction next)
        {
            TupleList tuple = new TupleList();
            while (IsReadable(c))
            {
                var temp = next(ref c);
                if(temp == null)
                {
                    break;
                }
                if(tuple.Count <= 0)
                {
                    tuple.Position = temp.Position;
                }
                tuple.Append(temp);
                if (!CheckToken(c, TokenType.List))
                {
                    break;
                }
                SkipSpaser(++c);
            }
            return tuple;
        }
    }
}
