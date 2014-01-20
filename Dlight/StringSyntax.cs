using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class NumberLiteral : AbstractSyntax
    {
        public string Value { get; set; }

        public override string Info()
        {
            return Value;
        }

        public override void CheckError(System.IO.TextWriter output)
        {
            dynamic number;
            if(!TryParse(out number))
            {
                output.WriteLineAsync(ErrorInfo() + "数値リテラルに使用できない文字が含まれています。");
            }
        }

        public bool TryParse(out dynamic number)
        {
            number = 0;
            bool skip = false;
            foreach(char v in Value)
            {
                if (skip)
                {
                    skip = false;
                }
                else
                {
                    number *= 10;
                }
                switch(v)
                {
                    case '0': number += 0; break;
                    case '1': number += 1; break;
                    case '2': number += 2; break;
                    case '3': number += 3; break;
                    case '4': number += 4; break;
                    case '5': number += 5; break;
                    case '6': number += 6; break;
                    case '7': number += 7; break;
                    case '8': number += 8; break;
                    case '9': number += 9; break;
                    case '_': skip = true; break;
                    default: return false;
                }
            }
            return true;
        }
    }
}
