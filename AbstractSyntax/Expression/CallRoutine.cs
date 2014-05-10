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
        private Scope _CallScope;
        private List<DataType> _ArgumentDataType;

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    var access = Access as IAccess;
                    _CallScope = access.Reference.TypeSelect(ArgumentDataType);
                }
                return _CallScope;
            }
        }

        public IReadOnlyList<DataType> ArgumentDataType
        {
            get 
            { 
                if(_ArgumentDataType != null)
                {
                    return _ArgumentDataType;
                }
                _ArgumentDataType = new List<DataType>();
                foreach (var v in Argument)
                {
                    var temp = v.DataType;
                    _ArgumentDataType.Add(temp);
                }
                return _ArgumentDataType;
            }
        }

        public override DataType DataType
        {
            get 
            {
                if (_CallScope is CalculatePragma || _CallScope is CastPragma)
                {
                    return ArgumentDataType[0];
                }
                else if(_CallScope is RoutineSymbol)
                {
                    var rout = (RoutineSymbol)_CallScope;
                    return rout.DataType;
                }
                else
                {
                    return _CallScope.DataType; //todo もっと適切な方法で型を取得する必要がある。
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
            if(ArgumentDataType.Count != 1)
            {
                return Root.Unknown;
            }
            return ArgumentDataType[0];
        }
    }
}
