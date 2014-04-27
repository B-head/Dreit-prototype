using AbstractSyntax;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DlightTest
{
    class TestData
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public bool Ignore { get; set; }
        public bool Explicit { get; set; }
        public bool NoExecute { get; set; }
        public string Code { get; set; }
        public List<CompileMessage> Message { get; set; }
        public int InfoCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public override string ToString()
        {
            return Code;
        }

        public static string CodeNormalize(string code)
        {
            if (code == null)
            {
                return null;
            }
            var temp = code.Trim().Normalize();
            return Regex.Replace(temp, @"\s+", " ");
        }
    }

    class CompileMessageEqualityComparer : EqualityComparer<CompileMessage>
    {
        public override bool Equals(CompileMessage x, CompileMessage y)
        {
            return x.Key == y.Key && x.Type == y.Type;
        }

        public override int GetHashCode(CompileMessage obj)
        {
            return obj.GetHashCode();
        }
    }
}
