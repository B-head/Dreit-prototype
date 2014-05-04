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
            var count = 0;
            TokenAction<DirectiveList> action = (s, t)=> ++count;
            var ret = cp.Begin<DirectiveList>().Text(action, "var").Text(action, "a").End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Text2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var count = 0;
            TokenAction<DirectiveList> action = (s, t) => ++count;
            var ret = cp.Begin<DirectiveList>().Text(action, "let").Text(action, "a").End();
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Type1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var count = 0;
            TokenAction<DirectiveList> action = (s, t) => ++count;
            var ret = cp.Begin<DirectiveList>().Type(action, TokenType.LetterStartString).Type(action, TokenType.LetterStartString).End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Type2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var count = 0;
            TokenAction<DirectiveList> action = (s, t) => ++count;
            var ret = cp.Begin<DirectiveList>().Type(action, TokenType.DigitStartString).Type(action, TokenType.LetterStartString).End();
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Transfer1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var count = 0;
            var element = new DirectiveList();
            ElementAction<DirectiveList> action = (s, t) => ++count;
            var ret = cp.Begin<DirectiveList>().Transfer(action, c => null, c => element).Transfer(action, c => element, c => null).End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Transfer2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new ChainParser(tc);
            var count = 0;
            ElementAction<DirectiveList> action = (s, t) => ++count;
            var ret = cp.Begin<DirectiveList>().Transfer(action, c => null, c => null).Transfer(action, c => null, c => null).End();
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }
    }
}
