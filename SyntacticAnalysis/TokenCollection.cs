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
        public TextPosition LastPosition { get; private set; }

        public TokenCollection(string text, string fileName, List<Token> tokenList, List<Token> errorToken, TextPosition lastPosition)
        {
            Text = text;
            FileName = fileName;
            _TokenList = tokenList;
            _ErrorToken = errorToken;
            LastPosition = lastPosition;
        }

        public string GetName()
        {
            var temp = Regex.Replace(FileName, @"\..*$", "");
            return Regex.Replace(temp, @"^.*/", "");
        }

        public bool IsReadable(int c)
        {
            return c < _TokenList.Count;
        }

        public Token Read(int c)
        {
            return _TokenList[c];
        }

        public int MoveNextToken(int c)
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
            return c;
        }

        public void AddError(int c)
        {
            _ErrorToken.Add(Read(c));
        }

        public TextPosition GetTextPosition(int c)
        {
            if (IsReadable(c))
            {
                return Read(c).Position;
            }
            else
            {
                return LastPosition;
            }
        }

        public bool CheckToken(int c, params TokenType[] type)
        {
            TokenType temp;
            return CheckToken(c, out temp, type);
        }

        public bool CheckToken(int c, out TokenType match, params TokenType[] type)
        {
            match = TokenType.Unknoun;
            if (!IsReadable(c))
            {
                return false;
            }
            Token t = Read(c);
            foreach (TokenType v in type)
            {
                if (t.Type == v)
                {
                    match = v;
                    return true;
                }
            }
            return false;
        }

        public bool CheckText(int c, params string[] text)
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
