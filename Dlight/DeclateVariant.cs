using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class DeclareVariant : Scope
    {
        public Identifier Ident { get; set; }
        public Identifier ExplicitType { get; set; }
        public FullName IdentType { get; set; }

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
                case 1: return ExplicitType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override string ElementInfo()
        {
            string temp = " (" + IdentType + ")";
            if (ExplicitType == null)
            {
                return base.ElementInfo() + Ident.Value + temp;
            }
            else
            {
                return base.ElementInfo() + Ident.Value + ":" + ExplicitType.Value + temp;
            }
        }

        public override void SpreadScope(Scope scope, Element parent)
        {
            Name = Ident.Value;
            base.SpreadScope(scope, parent);
        }

        public override void CheckDataType()
        {
            if (ExplicitType != null)
            {
                IdentType = NameResolution(ExplicitType.Value).FullName;
            }
            base.CheckDataType();
        }

        public override void CheckDataTypeAssign(FullName type)
        {
            if (IdentType == null)
            {
                IdentType = type;
            }
            base.CheckDataTypeAssign(type);
        }

        public override FullName GetDataType()
        {
            return IdentType;
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
            FullName type = GetDataType();
            Translator temp = trans.GenelateVariant(Scope.FullName, type);
            base.SpreadTranslate(temp);
        }
    }
}
