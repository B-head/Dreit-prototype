using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateRoutine : Scope
    {
        public IdentifierAccess Ident { get; set; }
        public TupleList Generic { get; set; }
        public TupleList Argument { get; set; }
        public Element ExplicitType { get; set; }
        public DirectiveList Block { get; set; }
        public List<DataType> ArgumentType { get; set; }
        public DataType ReturnType { get; set; }

        public DeclateRoutine()
        {
            ReturnType = new VoidSymbol();
        }

        public override bool IsVoidValue
        {
            get { return true; } //todo この代わりのプロパティが必要。
        }

        public override DataType DataType
        {
            get { return ReturnType; }
        }

        public override int Count
        {
            get { return 5; }
        }

        public override Element this[int index]
        {
            get
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
        }

        protected override string CreateName()
        {
            return Ident == null ? Name : Ident.Value;
        }

        internal override TypeMatchResult TypeMatch(List<DataType> type)
        {
            if (ArgumentType == null)
            {
                return TypeMatchResult.Unknown;
            }
            if (ArgumentType.Count != type.Count)
            {
                return TypeMatchResult.MissMatchCount;
            }
            else
            {
                for (int i = 0; i < ArgumentType.Count; i++)
                {
                    if (ArgumentType[i] != type[i])
                    {
                        return TypeMatchResult.MissMatchType;
                    }
                }
            }
            return TypeMatchResult.PerfectMatch;
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            var refer = new List<DataType>();
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
                if (ReturnType is VoidSymbol)
                {
                    ReturnType = ret.DataType;
                }
                else if(ReturnType != ret.DataType)
                {
                    CompileError("disagree-return-type");
                }
            }
            else
            {
                var ret = Block.FindElements<ReturnDirective>();
                if (ReturnType is VoidSymbol && ret.Count > 0)
                {
                    ReturnType = ret[0].DataType;
                }
                foreach(var v in ret)
                {
                    if (ReturnType != v.DataType)
                    {
                        CompileError("disagree-return-type");
                    }
                }
            }
        }
    }
}
