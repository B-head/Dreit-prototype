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
        public TupleList Argument { get; set; }
        public Scope CallScope { get; set; }
        private List<DataType> _ArgumentType { get; set; }

        public List<DataType> ArgumentType
        {
            get 
            { 
                if(_ArgumentType != null)
                {
                    return _ArgumentType;
                }
                _ArgumentType = new List<DataType>();
                foreach (var v in Argument)
                {
                    var temp = v.DataType;
                    _ArgumentType.Add(temp);
                }
                return _ArgumentType;
            }
        }

        public override DataType DataType
        {
            get 
            {
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
                    return ArgumentType[0];
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
                    case 1: return Argument;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            var access = Access as IAccess;
            CallScope = access.Reference.TypeSelect(ArgumentType);
            if(CallScope == null)
            {
                CompileError("unmatch-overroad");
            }
            /*var ret = CallScope.TypeMatch(ArgumentType);
            switch(ret)
            {
                case TypeMatchResult.Unknown: CompileError("原因不明の呼び出しエラーです。"); break;
                case TypeMatchResult.NotCallable: CompileError("関数ではありません。"); break;
                case TypeMatchResult.MissMatchCount: CompileError("引数の数が合っていません。"); break;
                case TypeMatchResult.MissMatchType: CompileError("引数の型が合っていません。"); break;
            }*/
        }

        public DataType GetCallType()
        {
            if(ArgumentType.Count == 0)
            {
                return Root.Unknown;
            }
            return ArgumentType[0];//todo こういう適当実装を減らしたいな～
        }
    }
}
