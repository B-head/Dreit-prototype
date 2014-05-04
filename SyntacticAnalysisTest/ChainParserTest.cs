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

        [TestCase(" test ", 4)]
        [TestCase(" \n ;\n\n ;;; ; test;; ", 16)]
        public void Ignore(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Ignore(TokenType.EndExpression, TokenType.LineTerminator).Text("test").End();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(expected));
        }

        [TestCase("test(a)b", 8)]
        [TestCase("test a b", 6)]
        [TestCase("test[a]b", 0)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void If(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .If().Type(TokenType.LeftParenthesis)
                .Than().Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis)
                .EndIf().Type(TokenType.LetterStartString).End();
            if(expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }

        [TestCase("test(a)b", 8)]
        [TestCase("test a b", 0)]
        [TestCase("test[a]b", 8)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void Else(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .If().Type(TokenType.LeftParenthesis)
                .Than().Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis)
                .Else().Type(TokenType.LeftBracket).Type(TokenType.LetterStartString).Type(TokenType.RightBracket)
                .EndIf().Type(TokenType.LetterStartString).End();
            if (expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }

        [TestCase("test(a)b", 8)]
        [TestCase("test a b", 8)]
        [TestCase("test[a]b", 8)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void ElseIf(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .If().Type(TokenType.LeftParenthesis)
                .Than().Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis)
                .ElseIf().Type(TokenType.LeftBracket)
                .Than().Type(TokenType.LetterStartString).Type(TokenType.RightBracket)
                .Else().Type(TokenType.LetterStartString)
                .EndIf().Type(TokenType.LetterStartString).End();
            if (expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }

        [TestCase("test a b", 8)]
        [TestCase("test 5 b", 8)]
        [TestCase("test * b", 0)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Or(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .Type(TokenType.LetterStartString).Or().Type(TokenType.DigitStartString)
                .Type(TokenType.LetterStartString).End();
            if (expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }

        [TestCase("test a b", 6)]
        [TestCase("test 5 b", 0)]
        [TestCase("test * b", 0)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Not(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .Not().Type(TokenType.DigitStartString)
                .Type(TokenType.LetterStartString).End();
            if (expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }

        [TestCase("test a b", 6)]
        [TestCase("test 5 b", 8)]
        [TestCase("test * b", 0)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Opt(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new ChainParser(tc);
            var ret = cp.Begin<DirectiveList>().Text("test")
                .Opt().Type(TokenType.DigitStartString)
                .Type(TokenType.LetterStartString).End();
            if (expected == 0)
            {
                Assert.That(ret, Is.Null);
            }
            else
            {
                Assert.That(ret, Is.Not.Null);
                Assert.That(ret.Position.Length, Is.EqualTo(expected));
            }
        }
    }
}
