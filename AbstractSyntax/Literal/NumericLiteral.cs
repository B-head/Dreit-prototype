using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;
using System.Numerics;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class NumericLiteral : Element
    {
        public string Integral { get; private set; }
        public string Fraction { get; private set; }
        private Scope _ReturnType;

        public NumericLiteral(TextPosition tp, string integral, string fraction)
            :base(tp)
        {
            Integral = integral;
            Fraction = fraction;
        }

        public override Scope ReturnType
        {
            get
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                if (string.IsNullOrEmpty(Fraction))
                {
                    _ReturnType = CurrentScope.NameResolution("Int32").FindDataType();
                }
                else
                {
                    _ReturnType = CurrentScope.NameResolution("Double").FindDataType();
                }
                return _ReturnType;
            }
        }

        protected override string ElementInfo
        {
            get
            {
                if (string.IsNullOrEmpty(Fraction))
                {
                    return Integral;
                }
                else
                {
                    return Integral + "." + Fraction;
                }
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            Parse(Integral, cmm);
            if (string.IsNullOrEmpty(Fraction))
            {
                Parse(Fraction, cmm);
            }
        }

        public dynamic Parse()
        {
            if (string.IsNullOrEmpty(Fraction))
            {
                return (int)Parse(Integral);
            }
            else
            {
                int count, b;
                return (double)Parse(Integral) + (double)Parse(Fraction, out count, out b) / Math.Pow(b, count);
            }
        }

        private BigInteger Parse(string text, CompileMessageManager cmm = null)
        {
            int count, b;
            return Parse(text, out count, out b, cmm);
        }

        private BigInteger Parse(string text, out int count, out int b, CompileMessageManager cmm = null)
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
                        if (cmm != null)
                        {
                            cmm.CompileError("number-parse-error", this);
                        }
                        return 0;
                    }
                    number += temp;
                    count++;
                }
            }
            return number;
        }

        private static int CheckPrefix(ref string text)
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
        
        private static int ToNum(char c)
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
