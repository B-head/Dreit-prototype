using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class CallRoutine : Element
    {
        public Element Access { get; set; }
        public TupleList Argument { get; set; }
        public Scope CallScope { get; set; }
        public List<DataType> ArgumentType { get; set; }

        public override DataType DataType
        {
            get 
            {
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
                    return ArgumentType[0];
                }
                else
                {
                    return Access.DataType;
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

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            var argType = new List<DataType>();
            foreach (var v in Argument)
            {
                var temp = v.DataType;
                argType.Add(temp);
            }
            ArgumentType = argType;
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
    }
}
