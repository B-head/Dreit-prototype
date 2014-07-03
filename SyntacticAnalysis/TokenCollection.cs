using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    public class TokenCollection
    {
        public string Text { get; private set; }
        public string FileName { get; private set; }
        private List<Token> _TokenList;
        public IReadOnlyList<Token> TokenList { get { return _TokenList; } }
        private List<Token> _ErrorToken;
        public IReadOnlyList<Token> ErrorToken { get { return _ErrorToken; } }
        public TextPosition FirstPosition { get; private set; }
        public TextPosition LastPosition { get; private set; }

        public TokenCollection(string text, string fileName, List<Token> tokenList, List<Token> errorToken, TextPosition lastPosition)
        {
            Text = text;
            FileName = fileName;
            _TokenList = tokenList;
            _ErrorToken = errorToken;
            FirstPosition = new TextPosition { File = fileName };
            LastPosition = lastPosition;
        }

        public string GetName()
        {
            var temp = Regex.Replace(FileName, @"\..*$", "");
            return Regex.Replace(temp, @"^.*/", "");
        }

        public bool IsReadable(int i)
        {
            return i < _TokenList.Count;
        }

        public Token Read(int i)
        {
            return _TokenList[i];
        }

        public void MoveNextToken(ref int i)
        {
            i++;
            while (IsReadable(i))
            {
                if (CheckToken(i, TokenType.LineTerminator))
                {
                    i++;
                    continue;
                }
                break;
            }
        }

        public void AddError(int i)
        {
            _ErrorToken.Add(Read(i));
        }

        public TextPosition GetTextPosition(int i)
        {
            if (IsReadable(i))
            {
                return Read(i).Position;
            }
            else
            {
                return LastPosition;
            }
        }

        public bool CheckToken(int i, params TokenType[] type)
        {
            TokenType temp;
            return CheckToken(i, out temp, type);
        }

        public bool CheckToken(int i, out TokenType match, params TokenType[] type)
        {
            match = TokenType.Unknoun;
            if (!IsReadable(i))
            {
                return false;
            }
            Token t = Read(i);
            foreach (TokenType v in type)
            {
                if (v == TokenType.LeftAssign || v == TokenType.RightAssign)
                {
                    if ((t.TokenType & v) != 0)
                    {
                        match = v;
                        return true;
                    }
                }
                else
                {
                    if (t.TokenType == v)
                    {
                        match = v;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckText(int i, params string[] text)
        {
            if (!IsReadable(i))
            {
                return false;
            }
            Token t = Read(i);
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
