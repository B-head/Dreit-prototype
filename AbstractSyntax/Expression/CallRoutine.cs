using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;

namespace AbstractSyntax.Expression
{
    public class CallRoutine : Element
    {
        public Element Access { get; set; }
        public TupleList Argument { get; set; }
        public List<Scope> ArgumentType { get; set; }

        public override bool IsVoidValue
        {
            get { return Access.Reference.DataType is VoidScope; }
        }

        public override Scope DataType
        {
            get 
            {
                if (Access.Reference is CalculatePragma || Access.Reference is CastPragma)
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

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Access;
                case 1: return Argument;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            var argType = new List<Scope>();
            foreach (var v in Argument)
            {
                var temp = v.DataType;
                argType.Add(temp);
            }
            ArgumentType = argType;
            var ret = Access.Reference.TypeMatch(ArgumentType);
            switch(ret)
            {
                case TypeMatchResult.Unknown: CompileError("原因不明の呼び出しエラーです。"); break;
                case TypeMatchResult.NotCallable: CompileError("関数ではありません。"); break;
                case TypeMatchResult.MissMatchCount: CompileError("引数の数が合っていません。"); break;
                case TypeMatchResult.MissMatchType: CompileError("引数の型が合っていません。"); break;
            }
        }
    }
}
