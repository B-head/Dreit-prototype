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

        public override void Translate()
        {
            Scope temp = Scope.NameResolution(Value);
            Trans.GenelateLoad(temp.GetFullName());
            base.Translate();
        }

        public override void TranslateAssign()
        {
            Scope temp = Scope.NameResolution(Value);
            Trans.GenelateStore(temp.GetFullName());
            base.TranslateAssign();
        }
    }

    class DeclareVariant : Scope
    {
        public Identifier Ident { get; set; }
        public Identifier DataType { get; set; }

        public override bool IsReference
        {
            get { return true; }
        }

        public override int ChildCount
        {
            get { return 2; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return DataType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override string ElementInfo()
        {
            if (DataType == null)
            {
                return base.ElementInfo() + Ident.Value;
            }
            else
            {
                return base.ElementInfo() + Ident.Value + ":" + DataType.Value;
            }
        }

        public override void SpreadScope(Scope scope, Element parent)
        {
            Name = Ident.Value;
            base.SpreadScope(scope, parent);
        }

        public override void SpreadTranslate(Translator trans)
        {
            Translator temp = trans.GenelateVariant(Scope, "Integer32");
            base.SpreadTranslate(temp);
        }
    }
}
