using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class CallRoutine : Element, ICaller
    {
        public Element Access { get; set; }
        public TupleList Arguments { get; set; }
        private Scope _CallScope;

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    var access = Access as IAccess;
                    if (access == null)
                    {
                        _CallScope = Root.Unknown;
                    }
                    else
                    {
                        _CallScope = access.Reference.TypeSelect(Arguments.GetDataTypes()).Call;
                    }
                }
                return _CallScope;
            }
        }

        public override DataType DataType
        {
            get 
            {
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
                    return Arguments.GetDataTypes()[0];
                }
                else if(CallScope is RoutineSymbol)
                {
                    var rout = (RoutineSymbol)CallScope;
                    return rout.DataType;
                }
                else
                {
                    return CallScope.DataType; //todo もっと適切な方法で型を取得する必要がある。
                }
            }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Access;
                    case 1: return Arguments;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if(CallScope == null)
            {
                CompileError("unmatch-overroad");
            }
            //var ret = CallScope.TypeMatch(ArgumentType);
            //switch(ret)
            //{
            //    case TypeMatchResult.Unknown: CompileError("原因不明の呼び出しエラーです。"); break;
            //    case TypeMatchResult.NotCallable: CompileError("関数ではありません。"); break;
            //    case TypeMatchResult.MissMatchCount: CompileError("引数の数が合っていません。"); break;
            //    case TypeMatchResult.MissMatchType: CompileError("引数の型が合っていません。"); break;
            //}
        }

        public DataType GetCallType()
        {
            if (Arguments.GetDataTypes().Count != 1)
            {
                return Root.Unknown;
            }
            return Arguments.GetDataTypes()[0];
        }
    }
}
