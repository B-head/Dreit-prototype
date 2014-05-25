using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class EchoDirective : Element
    {
        public Element Exp { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Exp;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if(Exp != null && Exp.DataType is VoidSymbol)
            {
                CompileError("invalid-void");
            }
        }
    }
}
