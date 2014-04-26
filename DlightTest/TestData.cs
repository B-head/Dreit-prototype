
namespace DlightTest
{
    class TestData
    {
        public string Code { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public bool Ignore { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
