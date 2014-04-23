using NUnit.Framework;
using NUnit.Framework.Constraints;
using SyntacticAnalysis;
using System;

namespace SyntacticAnalysisTest
{
    [TestFixture]
    public class TokenizerTest
    {
        private Tokenizer empty;

        [SetUp]
        public void SetUp()
        {
            empty = new Tokenizer("", "empty");
        }

        [TestCase("", 0, false)]
        [TestCase("abc", 0, true)]
        [TestCase("abc", 2, true)]
        [TestCase("abc", 3, false)]
        public void IsReadable(string text, int index, bool expected)
        {
            Tokenizer t = new Tokenizer(text, string.Empty);
            Assert.That(t.IsReadable(index), Is.EqualTo(expected));
        }
    }
}
