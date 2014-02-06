using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CilTranslate;

namespace AbstractSyntax
{
    public class Identifier : Element
    {
        public string Value { get; set; }
        public Element Refer { get; set; }

        public override bool IsReference
        {
            get { return true; }
        }

        protected override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }

        public override void CheckSemantic()
        {
            Refer = NameResolution(Value);
            if (Refer == null)
            {
                CompileError("このスコープで識別子 " + Value + " が宣言されていません。");
                return;
            }
            base.CheckSemantic();
        }

        public override Translator GetDataType()
        {
            return Refer.GetDataType();
        }

        public override void Translate()
        {
            Trans.GenelateLoad(Refer.Trans);
            base.Translate();
        }

        public override void TranslateAssign()
        {
            Trans.GenelateStore(Refer.Trans);
            base.TranslateAssign();
        }
    }
}
