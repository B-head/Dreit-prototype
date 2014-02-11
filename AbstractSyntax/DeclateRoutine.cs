using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclateRoutine : Scope
    {
        public bool IsImport { get; set; }
        public RoutineTranslator RoutineTrans { get; private set; }
        public Identifier Ident { get; set; }
        public Element GenericList { get; set; }
        public Element ArgumentList { get; set; }
        public Element ExplicitResultType { get; set; }
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

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void SpreadTranslate(Translator trans)
        {
            if (IsImport)
            {
                trans.ImportRoutine(FullPath);
                base.SpreadTranslate(trans);
            }
            else
            {
                RoutineTrans = trans.CreateRoutine(FullPath);
                base.SpreadTranslate(RoutineTrans);
            }
        }

        internal override void Translate(Translator trans)
        {
            base.Translate(RoutineTrans);
        }
    }
}
