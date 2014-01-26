using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class NumberLiteral : Syntax
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

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            dynamic number;
            if (!TryParse(out number))
            {
                manager.Error(ErrorInfo() + "数値リテラルに使用できない文字が含まれています。");
            }
            base.CheckSemantic(manager, scope);
        }

        public override void Translate(Translator trans, Scope<Element> scope)
        {
            dynamic number;
            TryParse(out number);
            trans.GenelateNumber(number);
            base.Translate(trans, scope);
        }

        public bool TryParse(out dynamic number)
        {
            number = 0;
            bool skip = false;
            foreach (char v in Integral)
            {
                if (skip)
                {
                    skip = false;
                }
                else
                {
                    number *= 10;
                }
                switch (v)
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
