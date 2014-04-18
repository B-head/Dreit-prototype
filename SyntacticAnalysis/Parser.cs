using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using AbstractSyntax;
using SyntacticAnalysis;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private delegate Element ParserFunction(ref int c);
        private delegate E ParserFunction<E>(ref int c) where E : Element;
        private List<Token> InputToken;
        private List<Token> ErrorToken;
        private Lexer Lexer;

        public Element Parse(Lexer lexer, string name)
        {
            InputToken = lexer.Token.ToList();
            ErrorToken = lexer.ErrorToken.ToList();
            Lexer = lexer;
            int c = -1;
            MoveNextToken(ref c);
            var p = GetTextPosition(c);
            DirectiveList exp = DirectiveList(ref c, true);
            if (exp.Count > 0)
            {
                exp.Position = SetTextLength(p, exp[exp.Count - 1].Position);
            }
            else
            {
                exp.Position = p;
            }
            return new DeclateModule { Name = name, ExpList = exp, ErrorToken = ErrorToken, Position = new TextPosition { File = lexer.FileName, Length = lexer.Text.Length } };
        }

        private bool IsReadable(int c)
        {
            return c < InputToken.Count;
        }

        private Token Read(int c)
        {
            return InputToken[c];
        }

        private void MoveNextToken(ref int c)
        {
            c++;
            while (IsReadable(c))
            {
                if (CheckToken(c, TokenType.LineTerminator))
                {
                    c++;
                    continue;
                }
                break;
            }
        }

        private void AddError(int c)
        {
            ErrorToken.Add(Read(c));
        }

        private TextPosition GetTextPosition(int c)
        {
            if (IsReadable(c))
            {
                return Read(c).Position;
            }
            else
            {
                return Lexer.LastPosition;
            }
        }

        private TextPosition SetTextLength(TextPosition first, TextPosition last)
        {
            first.Length = last.Length + last.Total - first.Total;
            return first;
        }

        private bool CheckToken(int c, params TokenType[] type)
        {
            TokenType temp;
            return CheckToken(c, out temp, type);
        }

        private bool CheckToken(int c, out TokenType match, params TokenType[] type)
        {
            match = TokenType.Unknoun;
            if(!IsReadable(c))
            {
                return false;
            }
            Token t = Read(c);
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
                MoveNextToken(ref c);
                Element right = next(ref c);
                left = new R { Left = left, Right = right, Operator = match, Position = SetTextLength(left.Position, right.Position) };
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
            MoveNextToken(ref c);
            Element right = RightAssociative<R>(ref c, next, type);
            return new R { Left = left, Right = right, Operator = match, Position = SetTextLength(left.Position, right.Position) };
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
                tuple.Append(temp);
                if (!CheckToken(c, TokenType.List))
                {
                    break;
                }
                MoveNextToken(ref c);
            }
            if(tuple.Count > 0)
            {
                tuple.Position = SetTextLength(tuple[0].Position, tuple[tuple.Count - 1].Position);
            }
            else
            {
                tuple.Position = GetTextPosition(c);
            }
            return tuple;
        }
    }
}
