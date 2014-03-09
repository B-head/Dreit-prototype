using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class CallRoutine : Element
    {
        public Element Access { get; set; }
        public TupleList Argument { get; set; }
        public List<Scope> ArgumentType { get; set; }
        private bool _IsVoidValue = false;

        public override bool IsVoidValue
        {
            get { return _IsVoidValue; }
        }

        public override Scope DataType
        {
            get 
            {
                if (Access.Reference is CalculatePragma)
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
            CheckCall((dynamic)Access.Reference);
        }

        private void CheckCall(Scope scope)
        {
            CompileError("関数ではありません。");
        }

        private void CheckCall(DeclateRoutine rout)
        {
            if(ArgumentType.Count != rout.ArgumentType.Count)
            {
                CompileError("引数の数が合っていません。");
            }
            else
            {
                for (int i = 0; i < ArgumentType.Count; i++)
                {
                    if(ArgumentType[i] != rout.ArgumentType[i])
                    {
                        CompileError("引数の型が合っていません。");
                    }
                }
                if(rout.ReturnType == null)
                {
                    _IsVoidValue = true;
                }
            }
        }

        private void CheckCall(CalculatePragma pragma)
        {
            if (ArgumentType.Count != 2)
            {
                CompileError("引数の数が合っていません。");
            }
            else if (ArgumentType[0] != ArgumentType[1])
            {
                CompileError("引数の型が合っていません。");
            }
        }

        private void CheckCall(DeclateClass cls)
        {
            if (ArgumentType.Count != 0)
            {
                CompileError("引数の数が合っていません。");
            }
        }
    }
}
