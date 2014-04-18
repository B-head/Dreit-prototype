using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax
{
    public class DeclateRoutine : Scope
    {
        public IdentifierAccess Ident { get; set; }
        public TupleList Generic { get; set; }
        public TupleList Argument { get; set; }
        public Element ExplicitType { get; set; }
        public DirectiveList Block { get; set; }
        public List<Scope> ArgumentType { get; set; }
        public Scope ReturnType { get; set; }

        public DeclateRoutine()
        {
            ReturnType = new VoidScope();
        }

        public override bool IsVoidValue
        {
            get { return true; }
        }

        public override Scope DataType
        {
            get { return ReturnType; }
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
                case 3: return ExplicitType;
                case 4: return Block;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? Name : Ident.Value;
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
            if (ExplicitType != null)
            {
                ReturnType = ExplicitType.DataType;
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if(Block.IsInline)
            {
                var ret = Block[0];
                if(ReturnType is VoidScope)
                {
                    ReturnType = ret.DataType;
                }
                else if(ReturnType != ret.DataType)
                {
                    CompileError("返り値の型が合っていません。");
                }
            }
            else
            {
                var ret = Block.FindElements<ReturnDirective>();
                if(ReturnType is VoidScope && ret.Count > 0)
                {
                    ReturnType = ret[0].DataType;
                }
                foreach(var v in ret)
                {
                    if (ReturnType != v.DataType)
                    {
                        CompileError("返り値の型が合っていません。");
                    }
                }
            }
        }
    }
}
