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
        public Scope Refer { get; private set; }

        internal override Scope DataType
        {
            get { return Refer.DataType; }
        }

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

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if (scope != null)
            {
                Refer = scope.NameResolution(Value);
            }
            if (Refer == null)
            {
                CompileError("識別子 " + Value + " が宣言されていません。");
            }
        }

        internal override void Translate(Translator trans)
        {
            trans.GenelateLoad(Refer.FullPath);
            base.Translate(trans);
        }

        internal override void TranslateAssign(Translator trans)
        {
            trans.GenelateStore(Refer.FullPath);
            base.TranslateAssign(trans);
        }
    }
}
