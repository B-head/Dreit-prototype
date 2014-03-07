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
        public TupleList Generic { get; set; }
        public TupleList Argument { get; set; }
        public Element ExplicitResultType { get; set; }
        public DirectiveList Block { get; set; }
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

        public override Element GetChild(int index)
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
            foreach (var v in Argument)
            {
                var temp = v.DataType;
                refer.Add(temp);
            }
            ArgumentType = refer;
            if (ExplicitResultType != null)
            {
                ReturnType = ExplicitResultType.DataType;
            }
        }

        internal override void PostSpreadTranslate(Translator trans)
        {
            FullPath returnType = ReturnType == null ? null : ReturnType.FullPath;
            RoutineTrans = trans.CreateRoutine(FullPath, returnType);
            base.PostSpreadTranslate(RoutineTrans);
            if (RoutineTrans != null)
            {
                RoutineTrans.SaveArgument();
            }
        }

        internal override void Translate(Translator trans)
        {
            if (Block != null)
            {
                Block.Translate(RoutineTrans);
            }
        }
    }
}
