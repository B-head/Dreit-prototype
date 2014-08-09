using System;
using NUnit.Framework;
using SyntacticAnalysis;
using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysisTest
{
    [TestFixture]
    public class SlimChainParserTest
    {
        [Test]
        public void Text1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            TokenAction action = t => ++count;
            var ret = cp.Begin.Text(action, "var").Text(action, "a").End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Text2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            TokenAction action = t => ++count;
            var ret = cp.Begin.Text(action, "let").Text(action, "a").End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Type1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            TokenAction action = t => ++count;
            var ret = cp.Begin.Type(action, TokenType.LetterStartString).Type(action, TokenType.LetterStartString).End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Type2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            TokenAction action = t => ++count;
            var ret = cp.Begin.Type(action, TokenType.DigitStartString).Type(action, TokenType.LetterStartString).End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void Transfer1()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            var element = new ExpressionList();
            ElementAction<Element> action = e => ++count;
            var ret = cp.Begin.Transfer(action, c => null, c => element).Transfer(action, c => element, c => null).End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Not.Null);
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Transfer2()
        {
            var tc = Lexer.Lex("var a", string.Empty);
            var cp = new SlimChainParser(tc);
            var count = 0;
            ElementAction<Element> action = e => ++count;
            var ret = cp.Begin.Transfer(action, c => null, c => null).Transfer(action, c => null, c => null).End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Null);
            Assert.That(count, Is.EqualTo(0));
        }

        [TestCase(" test ", 4)]
        [TestCase(" \n ;\n\n ;;; ; test;; ", 16)]
        public void Ignore(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Ignore(TokenType.EndExpression, TokenType.LineTerminator).Text("test").End(tp => new TupleList(tp, null));
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Position.Length, Is.EqualTo(expected));
        }

        [TestCase("test(a)b", 7)]
        [TestCase("test a b", 4)]
        [TestCase("test(a b", 0)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void If(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .If(icp => icp.Type(TokenType.LeftParenthesis))
                .Than(icp => icp.Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis))
                .End(tp => new TupleList(tp, null));
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

        [TestCase("test(a)b", 7)]
        [TestCase("test(a b", 0)]
        [TestCase("test a b", 0)]
        [TestCase("test[a]b", 7)]
        [TestCase("test[a b", 0)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void Else(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .If(icp => icp.Type(TokenType.LeftParenthesis))
                .Than(icp => icp.Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis))
                .Else(icp => icp.Type(TokenType.LeftBracket).Type(TokenType.LetterStartString).Type(TokenType.RightBracket))
                .End(tp => new TupleList(tp, null));
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

        [TestCase("test(a)b", 7)]
        [TestCase("test(a b", 0)]
        [TestCase("test a b", 6)]
        [TestCase("test[a]b", 7)]
        [TestCase("test[a b", 0)]
        [TestCase("(a)b", 0)]
        [TestCase("[a]b", 0)]
        [TestCase("a b", 0)]
        public void ElseIf(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .If(icp => icp.Type(TokenType.LeftParenthesis))
                .Than(icp => icp.Type(TokenType.LetterStartString).Type(TokenType.RightParenthesis))
                .ElseIf(icp => icp.Type(TokenType.LeftBracket))
                .Than(icp => icp.Type(TokenType.LetterStartString).Type(TokenType.RightBracket))
                .Else(icp => icp.Type(TokenType.LetterStartString))
                .End(tp => new TupleList(tp, null));
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
        [TestCase("test 5 b", 6)]
        [TestCase("test * b", 0)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Any(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .Any(
                    icp => icp.Type(TokenType.LetterStartString),
                    icp => icp.Type(TokenType.DigitStartString)
                )
                .End(tp => new TupleList(tp, null));
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

        [TestCase("test a b", 4)]
        [TestCase("test 5 b", 0)]
        [TestCase("test * b", 4)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Not(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .Not.Type(TokenType.DigitStartString)
                .End(tp => new TupleList(tp, null));
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

        [TestCase("test a b", 4)]
        [TestCase("test 5 b", 6)]
        [TestCase("test * b", 4)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Opt(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .Opt.Type(TokenType.DigitStartString)
                .End(tp => new TupleList(tp, null));
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

        [TestCase("test a b c d 5", 14)]
        [TestCase("test 5 b", 6)]
        [TestCase("test * b", 0)]
        [TestCase("a b", 0)]
        [TestCase("5 b", 0)]
        public void Loop(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var cp = new SlimChainParser(tc);
            var ret = cp.Begin.Text("test")
                .Loop(icp => icp.Not.Type(TokenType.DigitStartString), icp => 
                {
                    icp.Type(TokenType.LetterStartString);
                })
                .End(tp => new TupleList(tp, null));
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
