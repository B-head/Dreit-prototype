using AbstractSyntax;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SyntacticAnalysis;
using System;
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
            var token = (Token)typeof(Lexer).Invoke("DisjunctionLexer", tokenizer, new Lexer.LexerFunction[] { t => null, t => t.TakeToken(2, TokenType.Unknoun) });
            if (token == null)
            {
                Assert.Fail();
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo("ab"));
            }
        }

        [Test]
        public void DisjunctionLexer2()
        {
            var tokenizer = new Tokenizer("abc", string.Empty);
            var token = (Token)typeof(Lexer).Invoke("DisjunctionLexer", tokenizer, new Lexer.LexerFunction[] { t => null, t => null });
            Assert.That(token, Is.EqualTo(null));
        }

        [TestCase("\n", "\n", TokenType.LineTerminator)]
        [TestCase("\r", "\r", TokenType.LineTerminator)]
        [TestCase("\nabc", "\n", TokenType.LineTerminator)]
        [TestCase("\rabc", "\r", TokenType.LineTerminator)]
        [TestCase("\n\rabc", "\n\r", TokenType.LineTerminator)]
        [TestCase("\r\nabc", "\r\n", TokenType.LineTerminator)]
        [TestCase("abc\n", null, TokenType.Unknoun)]
        public void LineTerminator(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LineTerminator", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }

        [TestCase("   ", "   ", TokenType.WhiteSpace)]
        [TestCase("\t\t\t", "\t\t\t", TokenType.WhiteSpace)]
        [TestCase(" \t \t \t", " \t \t \t", TokenType.WhiteSpace)]
        [TestCase("   abc", "   ", TokenType.WhiteSpace)]
        [TestCase("\t\t\tabc", "\t\t\t", TokenType.WhiteSpace)]
        [TestCase(" \t \t \tabc", " \t \t \t", TokenType.WhiteSpace)]
        [TestCase("abc \t \t \t", null, TokenType.Unknoun)]
        public void WhiteSpace(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("WhiteSpace", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }

        [TestCase("/*abc*/def", "/*abc*/", TokenType.BlockComment)]
        [TestCase("/*ab/*cd*/ef*/gh", "/*ab/*cd*/ef*/", TokenType.BlockComment)]
        [TestCase("/*abcdef", "/*abcdef", TokenType.BlockComment)]
        [TestCase("abc/*def*/", null, TokenType.Unknoun)]
        public void BlockComment(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("BlockComment", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }

        [TestCase("//abc\nedf", "//abc", TokenType.LineCommnet)]
        [TestCase("#!abc\nedf", "#!abc", TokenType.LineCommnet)]
        [TestCase("//abcedf", "//abcedf", TokenType.LineCommnet)]
        [TestCase("#!abcedf", "#!abcedf", TokenType.LineCommnet)]
        [TestCase("abc//edf", null, TokenType.Unknoun)]
        [TestCase("abc#!edf", null, TokenType.Unknoun)]
        public void LineCommnet(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LineCommnet", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
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
        [TestCase(@"abc+def", @"abc", TokenType.LetterStartString)]
        [TestCase(@"abc\ def", @"abc\", TokenType.LetterStartString)]
        [TestCase(@"\+abcdef\-", @"\+abcdef\-", TokenType.LetterStartString)]
        [TestCase("123", null, TokenType.Unknoun)]
        [TestCase("0abc", null, TokenType.Unknoun)]
        public void LetterStartString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("LetterStartString", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }

        [TestCase("123", "123", TokenType.DigitStartString)]
        [TestCase("123 456", "123", TokenType.DigitStartString)]
        [TestCase("123abc", "123abc", TokenType.DigitStartString)]
        [TestCase("0x123abc", "0x123abc", TokenType.DigitStartString)]
        [TestCase("1_2_3", "1_2_3", TokenType.DigitStartString)]
        [TestCase("_1_2_3_", null, TokenType.Unknoun)]
        [TestCase("\\1_2_3\\", null, TokenType.Unknoun)]
        [TestCase("123\\456", "123", TokenType.DigitStartString)]
        public void DigitStartString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("DigitStartString", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }

        [TestCase("あいうえお", "あいうえお", TokenType.OtherString)]
        [TestCase("あいうえお かきくけこ", "あいうえお", TokenType.OtherString)]
        [TestCase("あいうえおabcde", "あいうえお", TokenType.OtherString)]
        [TestCase("abcdeあいうえお", null, TokenType.Unknoun)]
        public void OtherString(string text, string eText, TokenType eType)
        {
            var t = new Tokenizer(text, string.Empty);
            var token = (Token)typeof(Lexer).Invoke("OtherString", t);
            if (token == null)
            {
                Assert.That(null, Is.EqualTo(eText));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.Type, Is.EqualTo(eType));
            }
        }
    }
}
