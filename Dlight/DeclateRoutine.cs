using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class DeclateRoutine : Scope
    {
        public Identifier Ident { get; set; }
        public Element AttribuleList { get; set; }
        public Identifier ResultExplicitType { get; set; }
        public Element Block { get; set; }
        public FullName ResultType { get; set; }

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

        public override string ElementInfo()
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

        public override void SpreadScope(Scope scope, Element parent)
        {
            Name = Ident.Value;
            base.SpreadScope(scope, parent);
        }

        public override void SpreadTranslate(Translator trans)
        {
            Translator temp = trans.GenelateRoutine(Scope.FullName);
            base.SpreadTranslate(temp);
        }
    }
}
