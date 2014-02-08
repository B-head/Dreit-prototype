using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class DeclateRoutine : Scope
    {
        public Identifier Ident { get; set; }
        public Element AttribuleList { get; set; }
        public Identifier ResultExplicitType { get; set; }
        public Element Block { get; set; }
        public Translator ResultType { get; set; }

        public override int ChildCount
        {
            get { return 4; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return AttribuleList;
                case 2: return ResultExplicitType;
                case 3: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string ElementInfo()
        {
            string temp = " (" + ResultType + ")";
            if (ResultExplicitType == null)
            {
                return base.ElementInfo() + Ident.Value + temp;
            }
            else
            {
                return base.ElementInfo() + Ident.Value + ":" + ResultExplicitType.Value + temp;
            }
        }

        internal override Translator CreateTranslator(Translator trans)
        {
            return trans.CreateRoutine(Ident.Value);
        }
    }
}
