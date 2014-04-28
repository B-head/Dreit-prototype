using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddCount(int count)
        {
            Total += count;
            Row += count;
        }

        public void LineTerminate()
        {
            ++Line;
            Row = 0;
        }

        public TextPosition AlterLength(int length)
        {
            TextPosition temp = this;
            temp.Length = length;
            return temp;
        }

        public TextPosition AlterLength(TextPosition? other)
        {
            if(!other.HasValue)
            {
                return this;
            }
            return AlterLength(other.Value.Length + other.Value.Total - Total);
        }

        public static explicit operator TextPosition?(Element element)
        {
            if(element == null)
            {
                return null;
            }
            return element.Position;
        }
    }
}
