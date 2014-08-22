/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using NUnit.Framework;
using AbstractSyntax.SyntacticAnalysis;
using AbstractSyntax;

namespace AbstractSyntaxTest
{
    [TestFixture]
    public class TokenCollectionTest
    {
        [TestCase("test", "test")]
        [TestCase("test.dl", "test")]
        [TestCase("lib/test", "test")]
        [TestCase("user/lib/test.lib.dl", "test")]
        [TestCase("", "")]
        public void GetName(string fileName, string expected)
        {
            var tc = Lexer.Lex(string.Empty, fileName);
            Assert.That(tc.GetName(), Is.EqualTo(expected));
        }

        [TestCase("var a", 0, true)]
        [TestCase("var a", 1, true)]
        [TestCase("var a", 2, false)]
        [TestCase("", 0, false)]
        public void IsReadable(string text, int index, bool expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            Assert.That(tc.IsReadable(index), Is.EqualTo(expected));
        }

        [TestCase("var a", 0, "var")]
        [TestCase("var a", 1, "a")]
        public void Read(string text, int index, string expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var token = tc.Read(index);
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("var a", 1)]
        [TestCase("var \n a", 2)]
        [TestCase("var \n\n  \n\n \r a", 6)]
        public void MoveNextToken(string text, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            int i = 0;
            tc.MoveNextToken(ref i);
            Assert.That(i, Is.EqualTo(expected));
        }

        [TestCase("test(a, b)", 2, 5)]
        [TestCase("test(a, b)", 6, 10)]
        public void GetTextPosition(string text, int index, int expected)
        {
            var tc = Lexer.Lex(text, string.Empty);
            var p = tc.GetTextPosition(index);
            Assert.That(p.Total, Is.EqualTo(expected));
        }

        [TestCase("test(a, 33)", 2, new TokenType[] { TokenType.LetterStartString, TokenType.DigitStartString }, true, TokenType.LetterStartString)]
        [TestCase("test(a, 33)", 4, new TokenType[] { TokenType.LetterStartString, TokenType.DigitStartString }, true, TokenType.DigitStartString)]
        [TestCase("test(a, 33)", 3, new TokenType[] { TokenType.LetterStartString, TokenType.DigitStartString }, false, TokenType.Unknoun)]
        public void CheckToken(string text, int index, TokenType[] type, bool eRet, TokenType eMatch)
        {
            var tc = Lexer.Lex(text, string.Empty);
            TokenType match;
            bool ret = tc.CheckToken(index, out match, type);
            Assert.That(ret, Is.EqualTo(eRet));
            Assert.That(match, Is.EqualTo(eMatch));
        }

        [TestCase("test(a, 33)", 2, new string[] { "a", "33" }, true)]
        [TestCase("test(a, 33)", 4, new string[] { "a", "33" }, true)]
        [TestCase("test(a, 33)", 3, new string[] { "a", "33" }, false)]
        public void CheckText(string text, int index, string[] mText, bool eRet)
        {
            var tc = Lexer.Lex(text, string.Empty);
            bool ret = tc.CheckText(index, mText);
            Assert.That(ret, Is.EqualTo(eRet));
        }
    }
}
