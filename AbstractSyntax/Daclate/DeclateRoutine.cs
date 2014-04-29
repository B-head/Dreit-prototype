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

        public override List<DataType> ArgumentType
        {
            get
            {
                if(base.ArgumentType != null)
                {
                    return base.ArgumentType;
                }
                base.ArgumentType = new List<DataType>();
                foreach (var v in Argument)
                {
                    var temp = v.DataType;
                    base.ArgumentType.Add(temp);
                }
                return base.ArgumentType;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override DataType ReturnType
        {
            get
            {
                if(base.ReturnType != null)
                {
                    return base.ReturnType;
                }
                if (ExplicitType != null)
                {
                    base.ReturnType = ExplicitType.DataType;
                }
                else
                {
                    base.ReturnType = Root.Void;
                }
                if (Block.IsInline)
                {
                    var ret = Block[0];
                    if (base.ReturnType is VoidSymbol)
                    {
                        if (ret is ReturnDirective)
                        {
                            base.ReturnType = ((ReturnDirective)ret).Exp.DataType;
                        }
                        else
                        {
                            base.ReturnType = ret.DataType;
                        }
                    }
                    else if (base.ReturnType != ret.DataType)
                    {
                        CompileError("disagree-return-type");
                    }
                }
                else
                {
                    var ret = Block.FindElements<ReturnDirective>();
                    if (base.ReturnType is VoidSymbol && ret.Count > 0)
                    {
                        base.ReturnType = ret[0].Exp.DataType;
                    }
                    foreach (var v in ret)
                    {
                        if (base.ReturnType != v.Exp.DataType)
                        {
                            CompileError("disagree-return-type");
                        }
                    }
                }
                return base.ReturnType;
            }
            set
            {
                throw new NotSupportedException();
            }
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
    }
}
