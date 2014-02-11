using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class Identifier : Element
    {
        public string Value { get; set; }
        public Scope Refer { get; set; }

        internal override Scope AccessType
        {
            get { return Refer; }
        }

        public override bool IsReference
        {
            get { return true; }
        }

        protected override string AdditionalInfo()
        {
            return Value;
        }

        internal override void CheckDataType(Scope scope)
        {
            if (scope != null)
            {
                Refer = scope.NameResolution(Value);
            }
            if (Refer == null)
            {
                CompileError("識別子 " + Value + " が宣言されていません。");
            }
            else
            {
                DataType = Refer.DataType;
            }
        }

        internal override void Translate()
        {
            //GetTranslator().GenelateLoad(Refer.FullPath);
            base.Translate();
        }

        internal override void TranslateAssign()
        {
            //GetTranslator().GenelateStore(Refer.FullPath);
            base.TranslateAssign();
        }
    }
}
