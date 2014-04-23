using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace SyntacticAnalysis
{
    public partial class Lexer
    {
        private delegate Token LexerFunction(Tokenizer t);
        public string Text { get; private set; }
        public string FileName { get; private set; }
        public TextPosition LastPosition { get; private set; }
        private List<Token> _Token;
        public IReadOnlyList<Token> Token
        {
            get { return _Token; }
        }
        private List<Token> _ErrorToken;
        public IReadOnlyList<Token> ErrorToken
        {
            get { return _ErrorToken; }
        }

        public void Lex(string text, string fileName)
        {
            Text = text;
            FileName = fileName;
            var tokenList = new List<Token>();
            var errorToken = new List<Token>();
            var t = new Tokenizer(text, fileName);
            while (t.IsReadable())
            {
                LexPartion(t, tokenList, errorToken);
            }
            LastPosition = t.Position;
        }

        private TokenType LexPartion(Tokenizer t, List<Token> tokenList, List<Token> errorToken)
        {
            Token temp;
            temp = DisjunctionLexer
                (
                t,
                LineTerminator,
                WhiteSpace,
                BlockComment,
                LineCommnet
                );
            if (temp != null)
            {
                return temp.Type;
            }
            if (StringLiteral(t, tokenList, errorToken))
            {
                return TokenType.PlainText;
            }
            temp = DisjunctionLexer
                (
                t,
                TriplePunctuator,
                DoublePunctuator,
                SinglePunctuator,
                LetterStartString,
                DigitStartString
                );
            if (temp != null)
            {
                tokenList.Add(temp);
                return temp.Type;
            }
            errorToken.Add(OtherString(t));
            return TokenType.OtherString;
        }

        private Token DisjunctionLexer(Tokenizer t, params LexerFunction[] func)
        {
            foreach (var f in func)
            {
                var token = f(t);
                if (token != null)
                {
                    return token;
                }
            }
            return null;
        }
    }
}
