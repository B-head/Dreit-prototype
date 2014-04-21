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

        public TextPosition(string file, int total, int line, int row)
        {
            File = file;
            Total = total;
            Line = line;
            Row = row;
            Length = 0;
        }

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
