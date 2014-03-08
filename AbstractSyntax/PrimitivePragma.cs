using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class PrimitivePragma : DeclateClass
    {
        public PrimitivePragmaType Type { get; private set; }
        public DeclateVariant PrimVar { get; private set; }

        public PrimitivePragma(PrimitivePragmaType type)
        {
            Type = type;
            PrimVar = new DeclateVariant { Name = "@@value", _DataType = this };
        }

        public override int Count
        {
            get { return 6; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return Generic;
                case 2: return Inherit;
                case 3: return Block;
                case 4: return This;
                case 5: return PrimVar;
                default: throw new ArgumentOutOfRangeException();
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

    public enum PrimitivePragmaType
    {
        Root,
        Integer32,
    }
}
