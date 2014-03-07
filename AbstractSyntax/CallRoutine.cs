using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
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

        internal override Scope DataType
        {
            get { return Access.DataType.DataType; }
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

        internal override void CheckDataType(Scope scope)
        {
            base.CheckDataType(scope);
            var refer = new List<Scope>();
            foreach (var v in Argument)
            {
                var temp = v.DataType;
                refer.Add(temp);
            }
            ArgumentType = refer;
            var call = Access.DataType;
            if(call is DeclateRoutine)
            {
                CheckCall((DeclateRoutine)call);
            }
            else if(call is DeclateClass)
            {
                CheckCall((DeclateClass)call);
            }
        }

        private void CheckCall(DeclateRoutine rout)
        {
            if(rout == null)
            {
                CompileError("関数ではありません。");
            }
            else if(ArgumentType.Count != rout.ArgumentType.Count)
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

        private void CheckCall(DeclateClass cls)
        {
            if (ArgumentType.Count != 0)
            {
                CompileError("引数の数が合っていません。");
            }
        }

        internal override void Translate(Translator trans)
        {
            if (Argument != null)
            {
                Argument.Translate(trans);
            }
            var temp = Access as MemberAccess;
            if(temp != null)
            {
                temp.TranslateAccess(trans);
            }
            trans.GenerateCall(Access.DataType.FullPath);
        }
    }
}
