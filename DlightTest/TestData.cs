using System.Collections.Generic;
using AbstractSyntax;

namespace DlightTest
{
    class TestData
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public bool Ignore { get; set; }
        public bool Explicit { get; set; }
        public string Code { get; set; }
        public List<CompileMessage> Message { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
