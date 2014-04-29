using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateRoutine : RoutineSymbol
    {
        public TupleList Generic { get; set; }
        public TupleList Argument { get; set; }
        public Element ExplicitType { get; set; }
        public DirectiveList Block { get; set; }

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
            get { return 4; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Generic;
                    case 1: return Argument;
                    case 2: return ExplicitType;
                    case 3: return Block;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
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
            else
            {
                ReturnType = Root.Void;
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
                    if (ret is ReturnDirective)
                    {
                        ReturnType = ((ReturnDirective)ret).Exp.DataType;
                    }
                    else
                    {
                        ReturnType = ret.DataType;
                    }
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
                    ReturnType = ret[0].Exp.DataType;
                }
                foreach(var v in ret)
                {
                    if (ReturnType != v.Exp.DataType)
                    {
                        CompileError("disagree-return-type");
                    }
                }
            }
        }
    }
}
