using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclateOperator: Scope
    {
        public RoutineTranslator RoutineTrans { get; private set; }
        public TokenType Operator{ get; set; }
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
            get { return 4; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Generic;
                case 1: return Argument;
                case 2: return ExplicitResultType;
                case 3: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
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
