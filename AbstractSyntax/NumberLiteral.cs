using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using CilTranslate;

namespace AbstractSyntax
{
    public class NumberLiteral : Element
    {
        public string Integral { get; set; }
        public string Fraction { get; set; }

        protected override string ElementInfo()
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
            Parse(Integral);
            if(Fraction != null)
            {
                Parse(Fraction);
            }
            base.CheckSemantic();
        }

        public override Translator GetDataType()
        {
            if(Fraction == null)
            {
                return Trans.NameResolution("DlightObject").NameResolution("Integer32");
            }
            else
            {
                return Trans.NameResolution("DlightObject").NameResolution("Binary64");
            }
        }

        public override void Translate()
        {
            if (Fraction == null)
            {
                int number = (int)Parse(Integral);
                Trans.GenelatePrimitive(number);
                Trans.GenelateCall(GetDataType());
            }
            else
            {
                double number = (double)Parse(Integral);
                int count, b;
                number += (double)Parse(Fraction, out count, out b) / Math.Pow(b, count);
                Trans.GenelatePrimitive(number);
                Trans.GenelateCall(GetDataType());
            }
            base.Translate();
        }

        private BigInteger Parse(string text)
        {
            int count, b;
            return Parse(text, out count, out b);
        }

        private BigInteger Parse(string text, out int count, out int b)
        {
            b = CheckPrefix(ref text);
            count = 0;
            BigInteger number = 0;
            foreach (char v in text)
            {
                if (v == '_')
                {
                    continue;
                }
                checked
                {
                    number *= b;
                    int temp = ToNum(v);
                    if (temp >= b)
                    {
                        CompileError("数値リテラルに使用できない文字が含まれています。");
                        return 0;
                    }
                    number += temp;
                    count++;
                }
            }
            return number;
        }

        private int CheckPrefix(ref string text)
        {
            if(text.Length < 2)
            {
                return 10;
            }
            int result = 0;
            string temp = text.Substring(0, 2);
            switch(temp)
            {
                case "0b": result = 2; break;
                case "0B": result = 2; break;
                case "0o": result = 8; break;
                case "0O": result = 8; break;
                case "0d": result = 10; break;
                case "0D": result = 10; break;
                case "0h": result = 16; break;
                case "0H": result = 16; break;
                case "0x": result = 16; break;
                case "0X": result = 16; break;
                default: return 10;
            }
            text = text.Substring(2);
            return result;
        }
        
        private int ToNum(char c)
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
                default: return int.MaxValue;
            }
        }
    }
}
