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
            if (string.IsNullOrEmpty(File))
            {
                return string.Format("<empty>()");
            }
            else
            {
                return string.Format("{0}({1}, {2})", File, Line, Row);
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
