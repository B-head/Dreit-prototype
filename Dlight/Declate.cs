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

        public override string GetDataType()
        {
            return Scope.NameResolution(Value).GetDataType();
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
        public string TypeFullName { get; set; }

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
            string temp = " (" + TypeFullName + ")";
            if (DataType == null)
            {
                return base.ElementInfo() + Ident.Value + temp;
            }
            else
            {
                return base.ElementInfo() + Ident.Value + ":" + DataType.Value + temp;
            }
        }

        public override void SpreadScope(Scope scope, Element parent)
        {
            Name = Ident.Value;
            base.SpreadScope(scope, parent);
        }

        public override void CheckDataType()
        {
            if (DataType != null)
            {
                TypeFullName = NameResolution(DataType.Value).GetFullName();
            }
            base.CheckDataType();
        }

        public override void CheckDataTypeAssign(string type)
        {
            if(TypeFullName == null)
            {
                TypeFullName = type;
            }
            base.CheckDataTypeAssign(type);
        }

        public override string GetDataType()
        {
            return TypeFullName;
        }

        public override void Translate()
        {
            Ident.Translate();
        }

        public override void TranslateAssign()
        {
            Ident.TranslateAssign();
        }

        public override void SpreadTranslate(Translator trans)
        {
            string type = GetDataType();
            Translator temp = trans.GenelateVariant(Scope, type);
            base.SpreadTranslate(temp);
        }
    }
}
