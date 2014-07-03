using AbstractSyntax;
using NUnit.Framework;
using SyntacticAnalysis;

namespace SyntacticAnalysisTest
{
    [TestFixture]
    public class TokenizerTest
    {
        [TestCase("", 0, false)]
        [TestCase("abc", 0, true)]
        [TestCase("abc", 2, true)]
        [TestCase("abc", 3, false)]
        public void IsReadable(string text, int index, bool expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.IsReadable(index), Is.EqualTo(expected));
        }

        [TestCase("abc", 0, 'a')]
        [TestCase("abc", 1, 'b')]
        [TestCase("abc", 2, 'c')]
        public void Read(string text, int index, char expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.Read(index), Is.EqualTo(expected));
        }

        [TestCase("abcde", 0, 5, "abcde")]
        [TestCase("abcde", 2, 1, "c")]
        [TestCase("abcde", 3, 3, "")]
        [TestCase("abcde", 0, 0, "")]
        public void Read(string text, int index, int length, string expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.Read(index, length), Is.EqualTo(expected));
        }

        [TestCase("c", "abcde", true)]
        [TestCase("z", "abcde", false)]
        public void MatchAny(string text, string list, bool expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.MatchAny(0, list), Is.EqualTo(expected));
        }

        [TestCase("c", 'h', 'm', false)]
        [TestCase("h", 'h', 'm', true)]
        [TestCase("k", 'h', 'm', true)]
        [TestCase("m", 'h', 'm', true)]
        [TestCase("v", 'h', 'm', false)]
        public void MatchRange(string text, char start, char end, bool expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.MatchRange(0, start, end), Is.EqualTo(expected));
        }

        [TestCase("abcdefg", 3, TokenType.PlainText, "abc", false)]
        [TestCase("abcdefg", 7, TokenType.PlainText, "abcdefg", false)]
        [TestCase("abcdefg", 0, TokenType.PlainText, "", false)]
        [TestCase("\n\r", 2, TokenType.LineTerminator, "\n\r", true)]
        public void TakeToken(string text, int length, TokenType type, string eText, bool eLine)
        {
            Tokenizer t = new Tokenizer(text, "file");
            var token = t.TakeToken(length, type);
            if (!token)
            {
                Assert.That(token, Is.EqualTo(Token.Empty));
            }
            else
            {
                Assert.That(token.Text, Is.EqualTo(eText));
                Assert.That(token.TokenType, Is.EqualTo(type));
                Assert.That(token.Position.File, Is.EqualTo("file"));
                Assert.That(token.Position.Total, Is.EqualTo(0));
                Assert.That(token.Position.Row, Is.EqualTo(0));
                Assert.That(token.Position.Length, Is.EqualTo(length));
                Assert.That(token.Position.Line, Is.EqualTo(1));
                Assert.That(t.Position.Total, Is.EqualTo(length));
                Assert.That(t.Position.Row, Is.EqualTo(eLine ? 0 : length));
                Assert.That(t.Position.Length, Is.EqualTo(0));
                Assert.That(t.Position.Line, Is.EqualTo(eLine ? 2 : 1));
            }
        }
    }
}
