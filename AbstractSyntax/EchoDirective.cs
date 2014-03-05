using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class EchoDirective : Element
    {
        public Element Exp { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return Exp;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal override void Translate(Translator trans)
        {
            base.Translate(trans);
            trans.GenerateControl(CodeType.Echo);
            trans.GenerateControl(CodeType.Void);
        }
    }
}
