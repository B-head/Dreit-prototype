using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class Identifier : Element
    {
        public string Value { get; set; }

        public override bool IsReference
        {
            get { return true; }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }

        public override void CheckSemantic()
        {
            Scope temp = Scope.NameResolution(Value);
            if (temp == null)
            {
                CompileError("このスコープで識別子 " + Value + " が宣言されていません。");
                return;
            }
            base.CheckSemantic();
        }

        public override FullName GetDataType()
        {
            return Scope.NameResolution(Value).GetDataType();
        }

        public override void Translate()
        {
            Scope temp = Scope.NameResolution(Value);
            Trans.GenelateLoad(temp.FullName);
            base.Translate();
        }

        public override void TranslateAssign()
        {
            Scope temp = Scope.NameResolution(Value);
            Trans.GenelateStore(temp.FullName);
            base.TranslateAssign();
        }
    }
}
