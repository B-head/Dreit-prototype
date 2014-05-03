using System;
using NUnit.Framework;
using SyntacticAnalysis;
using AbstractSyntax;

namespace SyntacticAnalysisTest
{
    [TestFixture]
    public class ChainParserTest
    {
        [Test]
        public void Text1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("var").Text("a").End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
        }

        [Test]
        public void Text2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("let").Text("a").End();
            Assert.That(ret, Is.Null);
        }

        [Test]
        public void Type1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Type(TokenType.LetterStartString).Type(TokenType.LetterStartString).End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
        }

        [Test]
        public void Type2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Type(TokenType.DigitStartString).Type(TokenType.LetterStartString).End();
            Assert.That(ret, Is.Null);
        }
    }
}
