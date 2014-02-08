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

        public override bool IsReference
        {
            get { return true; }
        }

        protected override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }

        internal override void CheckSemantic()
        {
            Refer = Scope.NameResolution(Value);
            if (Refer == null)
            {
                CompileError("このスコープで識別子 " + Value + " が宣言されていません。");
                return;
            }
            base.CheckSemantic();
        }

        internal override Translator GetDataType()
        {
            return Refer.GetDataType();
        }

        internal override void Translate()
        {
            Trans.GenelateLoad(Refer.Trans);
            base.Translate();
        }

        internal override void TranslateAssign()
        {
            Trans.GenelateStore(Refer.Trans);
            base.TranslateAssign();
        }
    }
}
