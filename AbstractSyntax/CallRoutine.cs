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
        public Element Argument { get; set; }
        public List<Scope> ArgumentType { get; set; }
        private bool IsVoidReturn = false;

        public override int Count
        {
            get { return 2; }
        }

        public override Element Child(int index)
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
            if (Argument is TupleList)
            {
                foreach (var v in Argument)
                {
                    var temp = v.DataType;
                    refer.Add(temp);
                }
            }
            else if (Argument != null)
            {
                var temp = Argument.DataType;
                refer.Add(temp);
            }
            ArgumentType = refer;
            DeclateRoutine rout = Access.DataType as DeclateRoutine;
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
                    IsVoidReturn = true;
                }
            }
        }

        internal override void Translate(Translator trans)
        {
            Argument.Translate(trans);
            trans.GenerateCall(Access.DataType.FullPath);
            if(IsVoidReturn)
            {
                trans.GenerateControl(CodeType.Void);
            }
        }
    }
}
