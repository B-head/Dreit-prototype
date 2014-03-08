using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class CalculatePragma : DeclateOperator
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
            Argument = new TupleList();
        }

        internal void PragmaTranslate(Translator trans, TupleList argument)
        {
            foreach(var v in argument)
            {
                v.Translate(trans);
                var value = v.DataType.DataType.NameResolution("@@value");
                trans.GenerateLoad(value.FullPath);
            }
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

        internal override void PreSpreadTranslate(Translator trans)
        {

        }

        internal override void PostSpreadTranslate(Translator trans)
        {

        }

        internal override void Translate(Translator trans)
        {

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
