using System;

namespace AbstractSyntax
{
    [Serializable]
    public struct TextPosition
    {
        public string File;
        public int Total;
        public int Line;
        public int Row;
        public int Length;

        public override string ToString()
        {
            if (File == null || File == string.Empty)
            {
                return "<empty>()";
            }
            else
            {
                return File + "(" + Line + "," + Row + ")";
            }
        }
    }
}
