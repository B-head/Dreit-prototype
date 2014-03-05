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
        public RoutineTranslator RoutineTrans { get; private set; }
        public Identifier Ident { get; set; }
        public Element Generic { get; set; }
        public Element Argument { get; set; }
        public Element ExplicitResultType { get; set; }
        public Element Block { get; set; }
        public List<Scope> ArgumentType { get; set; }
        public Scope ReturnType { get; set; }

        public override bool IsVoidValue
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 5; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return Generic;
                case 2: return Argument;
                case 3: return ExplicitResultType;
                case 4: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            var refer = new List<Scope>();
            if (Argument is TupleList)
            {
                foreach (var v in Argument)
                {
                    var temp = v.DataType;
                    refer.Add(temp);
                }
            }
            else if (Argument != null)
            {
                var temp = Argument.DataType;
                refer.Add(temp);
            }
            ArgumentType = refer;
            ReturnType = ExplicitResultType.DataType;
        }

        internal override void SpreadTranslate(Translator trans)
        {
            if (IsImport)
            {
                base.SpreadTranslate(trans);
            }
            else
            {
                FullPath returnType = ReturnType == null ? null : ReturnType.FullPath;
                RoutineTrans = trans.CreateRoutine(FullPath, returnType);
                base.SpreadTranslate(RoutineTrans);
                RoutineTrans.SaveArgument();
            }
        }

        internal override void Translate(Translator trans)
        {
            Block.Translate(RoutineTrans);
        }
    }
}
