using AbstractSyntax;
using NUnit.Framework;
using SyntacticAnalysis;
using System.Collections.Generic;
using TestUtility;

namespace SyntacticAnalysisTest
{
    [TestFixture]
    public class LexerTest
    {
        [Test]
        public void DisjunctionLexer1()
        {
            var tokenizer = new Tokenizer("abc", string.Empty);
            var token = (Token)typeof(Lexer).Invoke("DisjunctionLexer", tokenizer, new Lexer.LexerFunction[] { t => Token.Empty, t => t.TakeToken(2, TokenType.Unknoun) });
            Assert.That(token.Text, Is.EqualTo("ab"));
        }

        [Test]
        public void DisjunctionLexer2()
        {
            var tokenizer = new Tokenizer("abc", string.Empty);
            var token = (Token)typeof(Lexer).Invoke("DisjunctionLexer", tokenizer, new Lexer.LexerFunction[] { t => Token.Empty, t => Token.Empty });
            Assert.That(token, Is.EqualTo(Token.Empty));
        }

        [TestCase("\n", "\n", TokenType.LineTerminator)]
        [TestCase("\r", "\r", TokenType.LineTerminator)]
        [TestCase("\nabc", "\n", TokenType.LineTerminator)]
        [TestCase("\rabc", "\r", TokenType.LineTerminator)]
        [TestCase("\n\rabc", "\n\r", TokenType.LineTerminator)]
        [TestCase("\r\nabc", "\r\n", TokenType.LineTerminator)]
        [TestCase("abc\n", "", TokenType.Unknoun)]
        public void LineTerminator(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LineTerminator", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }

        [TestCase("   ", "   ", TokenType.WhiteSpace)]
        [TestCase("\t\t\t", "\t\t\t", TokenType.WhiteSpace)]
        [TestCase(" \t \t \t", " \t \t \t", TokenType.WhiteSpace)]
        [TestCase("   abc", "   ", TokenType.WhiteSpace)]
        [TestCase("\t\t\tabc", "\t\t\t", TokenType.WhiteSpace)]
        [TestCase(" \t \t \tabc", " \t \t \t", TokenType.WhiteSpace)]
        [TestCase("abc \t \t \t", "", TokenType.Unknoun)]
        public void WhiteSpace(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("WhiteSpace", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
    }

        [TestCase("/*abc*/def", "/*abc*/", TokenType.BlockComment)]
        [TestCase("/*ab/*cd*/ef*/gh", "/*ab/*cd*/ef*/", TokenType.BlockComment)]
        [TestCase("/*abcdef", "/*abcdef", TokenType.BlockComment)]
        [TestCase("abc/*def*/", "", TokenType.Unknoun)]
        public void BlockComment(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("BlockComment", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }

        [TestCase("//abc\nedf", "//abc", TokenType.LineCommnet)]
        [TestCase("#!abc\nedf", "#!abc", TokenType.LineCommnet)]
        [TestCase("//abcedf", "//abcedf", TokenType.LineCommnet)]
        [TestCase("#!abcedf", "#!abcedf", TokenType.LineCommnet)]
        [TestCase("abc//edf", "", TokenType.Unknoun)]
        [TestCase("abc#!edf", "", TokenType.Unknoun)]
        public void LineCommnet(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LineCommnet", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }

        [TestCase("'abc'def", new string[] { "'", "abc", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase("\"abc\"def", new string[] { "\"", "abc", "\"" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase("`abc`def", new string[] { "`", "abc", "`" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase("'abcdef", new string[] { "'", "abcdef" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText })]
        [TestCase(@"'ab\'cd\'ef'", new string[] { "'", @"ab\'cd\'ef", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase(@"'ab\\cd\\ef'", new string[] { "'", @"ab\\cd\\ef", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase("'abc{ def }ghi'", new string[] { "'", "abc", "{", "def", "}", "ghi", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.LeftBrace, TokenType.LetterStartString, TokenType.RightBrace, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase(@"'abc\{ def }ghi'", new string[] { "'", @"abc\{ def }ghi", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.PlainText, TokenType.QuoteSeparator })]
        [TestCase("'{ abc }'", new string[] { "'", "{", "abc", "}", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.LeftBrace, TokenType.LetterStartString, TokenType.RightBrace, TokenType.QuoteSeparator })]
        [TestCase("'{ '{ abc }' }' }'", new string[] { "'", "{", "'", "{", "abc", "}", "'", "}", "'" }, new TokenType[] { TokenType.QuoteSeparator, TokenType.LeftBrace, TokenType.QuoteSeparator, TokenType.LeftBrace, TokenType.LetterStartString, TokenType.RightBrace, TokenType.QuoteSeparator, TokenType.RightBrace, TokenType.QuoteSeparator })]
        [TestCase("abc'def'", new string[] { }, new TokenType[] { } )]
        public void StringLiteral(string text, string[] eTexts, TokenType[] eTypes)
        {
            var t = new Tokenizer(text, string.Empty);
            var tokenList = new List<Token>();
            var errorToken = new List<Token>();
            var result = (bool)typeof(Lexer).Invoke("StringLiteral", t, tokenList, errorToken);
            Assert.That(result, Is.EqualTo(eTexts.Length > 0));
            Assert.That(tokenList, Is.All.Not.Null);
            Assert.That(List.Map(tokenList).Property("Text"), Is.EqualTo(eTexts));
            Assert.That(List.Map(tokenList).Property("Type"), Is.EqualTo(eTypes));
            Assert.That(errorToken.Count, Is.EqualTo(0));
        }

        [TestCase("{ }", new string[] { "{", "}" }, new TokenType[] { TokenType.LeftBrace, TokenType.RightBrace })]
        [TestCase("{ } abc", new string[] { "{", "}" }, new TokenType[] { TokenType.LeftBrace, TokenType.RightBrace })]
        [TestCase("{ 1 + 1 } abc", new string[] { "{", "1", "+", "1", "}" }, new TokenType[] { TokenType.LeftBrace, TokenType.DigitStartString, TokenType.Add, TokenType.DigitStartString, TokenType.RightBrace })]
        [TestCase("{ loop{ } } }", new string[] { "{", "loop", "{", "}", "}" }, new TokenType[] { TokenType.LeftBrace, TokenType.LetterStartString, TokenType.LeftBrace, TokenType.RightBrace, TokenType.RightBrace })]
        [TestCase("abc { }", new string[] { }, new TokenType[] { })]
        public void BuiltInExpression(string text, string[] eTexts, TokenType[] eTypes)
        {
            var t = new Tokenizer(text, string.Empty);
            var tokenList = new List<Token>();
            var errorToken = new List<Token>();
            typeof(Lexer).Invoke("BuiltInExpression", t, tokenList, errorToken);
            Assert.That(tokenList, Is.All.Not.Null);
            Assert.That(List.Map(tokenList).Property("Text"), Is.EqualTo(eTexts));
            Assert.That(List.Map(tokenList).Property("Type"), Is.EqualTo(eTypes));
            Assert.That(errorToken.Count, Is.EqualTo(0));
        }

        [TestCase("abc", "abc", TokenType.LetterStartString)]
        [TestCase("abc def", "abc", TokenType.LetterStartString)]
        [TestCase("ABC", "ABC", TokenType.LetterStartString)]
        [TestCase("abc123", "abc123", TokenType.LetterStartString)]
        [TestCase("a_b_c", "a_b_c", TokenType.LetterStartString)]
        [TestCase("_a_b_c_", "_a_b_c_", TokenType.LetterStartString)]
        [TestCase("_", "_", TokenType.LetterStartString)]
        [TestCase(@"abc\\def", @"abc\\def", TokenType.LetterStartString)]
        [TestCase(@"abc\+def", @"abc\+def", TokenType.LetterStartString)]
        [TestCase(@"abc\ndef", @"abc\ndef", TokenType.LetterStartString)]
        [TestCase(@"abc+def", @"abc", TokenType.LetterStartString)]
        [TestCase(@"abc\ def", @"abc\", TokenType.LetterStartString)]
        [TestCase(@"\+abcdef\-", @"\+abcdef\-", TokenType.LetterStartString)]
        [TestCase("123", "", TokenType.Unknoun)]
        [TestCase("0abc", "", TokenType.Unknoun)]
        public void LetterStartString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LetterStartString", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }

        [TestCase("123", "123", TokenType.DigitStartString)]
        [TestCase("123 456", "123", TokenType.DigitStartString)]
        [TestCase("123abc", "123abc", TokenType.DigitStartString)]
        [TestCase("0x123abc", "0x123abc", TokenType.DigitStartString)]
        [TestCase("1_2_3", "1_2_3", TokenType.DigitStartString)]
        [TestCase("_1_2_3_", "", TokenType.Unknoun)]
        [TestCase("\\1_2_3\\", "", TokenType.Unknoun)]
        [TestCase("123\\456", "123", TokenType.DigitStartString)]
        public void DigitStartString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("DigitStartString", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }

        [TestCase("あいうえお", "あいうえお", TokenType.OtherString)]
        [TestCase("あいうえお かきくけこ", "あいうえお", TokenType.OtherString)]
        [TestCase("あいうえおabcde", "あいうえお", TokenType.OtherString)]
        [TestCase("abcdeあいうえお", "", TokenType.Unknoun)]
        public void OtherString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("OtherString", t);
            Assert.That(token.Text, Is.EqualTo(eText));
            Assert.That(token.Type, Is.EqualTo(eType));
        }
    }
}
