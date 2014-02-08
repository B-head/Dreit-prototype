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
        public Element GenericList { get; set; }
        public Element ArgumentList { get; set; }
        public Identifier ExplicitResultType { get; set; }
        public Element Block { get; set; }
        public Scope ResultType { get; set; }

        public override int ChildCount
        {
            get { return 5; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return GenericList;
                case 2: return ArgumentList;
                case 3: return ExplicitResultType;
                case 4: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string ElementInfo()
        {
            string temp = " (" + ResultType + ")";
            if (ExplicitResultType == null)
            {
                return base.ElementInfo() + Ident.Value + temp;
            }
            else
            {
                return base.ElementInfo() + Ident.Value + ":" + ExplicitResultType.Value + temp;
            }
        }

        internal override Translator CreateTranslator(Translator trans)
        {
            return trans.CreateRoutine(FullPath);
        }
    }
}
