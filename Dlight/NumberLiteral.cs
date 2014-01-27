using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class NumberLiteral : Element
    {
        public string Integral { get; set; }
        public string Fraction { get; set; }

        public override string ElementInfo()
        {
            if (Fraction == null)
            {
                return base.ElementInfo() + Integral;
            }
            else
            {
                return base.ElementInfo() + Integral + "." + Fraction;
            }
        }

        public override void CheckSemantic()
        {
            bool unchar, overflow;
            Parse(out unchar, out overflow);
            if (unchar)
            {
                CompileError("数値リテラルに使用できない文字が含まれています。");
            }
            base.CheckSemantic();
        }

        public override void Translate()
        {
            int number = (int)Parse();
            Trans.GenelateConstant(number);
            base.Translate();
        }

        private ulong Parse(uint b = 10)
        {
            bool unchar, overflow;
            return Parse(out unchar, out overflow, b);
        }

        private ulong Parse(out bool unchar, out bool overflow, uint b = 10)
        {
            unchar = false; overflow = false;
            ulong number = 0;
            foreach (char v in Integral)
            {
                if(v == '_')
                {
                    continue;
                }
                number *= b;
                uint temp = ToNum(v);
                if(temp >= b)
                {
                    unchar = true;
                    return 0;
                }
                number += temp;
            }
            return number;
        }
        
        private uint ToNum(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 12;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
                default: return uint.MaxValue;
            }
        }
    }
}
