using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    class Binomial : Syntax
    {
        public Syntax Left { get; set; }
        public Syntax Right { get; set; }
        public SyntaxType Operation { get; set; }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Left;
                    case 1: return Right;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Enum.GetName(typeof(SyntaxType), Operation);
        }

        public override void CheckSemantic(TextWriter output)
        {
            if(Left == null)
            {
                output.WriteLineAsync(ErrorInfo() + "左辺式が必要です。");
            }
            if (Right == null)
            {
                output.WriteLineAsync(ErrorInfo() + "右辺式が必要です。");
            }
        }

        public override void Translate(Translator trans)
        {
            base.Translate(trans);
            trans.GenelateBinomial(typeof(DlightObject.Integer32), Operation);
        }
    }

    class NumberLiteral : Syntax
    {
        public string Value { get; set; }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }

        public override void CheckSemantic(System.IO.TextWriter output)
        {
            dynamic number;
            if (!TryParse(out number))
            {
                output.WriteLineAsync(ErrorInfo() + "数値リテラルに使用できない文字が含まれています。");
            }
        }

        public override void Translate(Translator trans)
        {
            dynamic number;
            TryParse(out number);
            trans.GenelateNumber(number);
        }

        public bool TryParse(out dynamic number)
        {
            number = 0;
            bool skip = false;
            foreach (char v in Value)
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
