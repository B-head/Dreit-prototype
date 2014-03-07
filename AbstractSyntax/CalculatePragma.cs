using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class CalculatePragma : Pragma
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
        }

        internal override void Translate(Translator trans)
        {
            switch(Type)
            {
                case CalculatePragmaType.Add: trans.GenerateControl(CodeType.Add); break;
                case CalculatePragmaType.Sub: trans.GenerateControl(CodeType.Sub); break;
                case CalculatePragmaType.Mul: trans.GenerateControl(CodeType.Mul); break;
                case CalculatePragmaType.Div: trans.GenerateControl(CodeType.Div); break;
                case CalculatePragmaType.Mod: trans.GenerateControl(CodeType.Mod); break;
                default: throw new Exception();
            }
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
    }
}
