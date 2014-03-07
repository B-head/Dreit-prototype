using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclateVariant : Scope
    {
        public Identifier Ident { get; set; }
        public Element ExplicitVariantType { get; set; }
        private Scope _DataType;

        internal override Scope DataType
        {
            get { return _DataType; }
        }

        public override bool IsAssignable
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return ExplicitVariantType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void SetDataType(Scope type)
        {
            _DataType = type;
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if (ExplicitVariantType != null)
            {
                _DataType = ExplicitVariantType.DataType;
            }
        }

        internal override void PostSpreadTranslate(Translator trans)
        {
            trans.CreateVariant(FullPath, DataType.FullPath);
            base.PostSpreadTranslate(trans);
        }

        internal override void Translate(Translator trans)
        {
            Ident.Translate(trans);
        }

        internal override void TranslateAssign(Translator trans)
        {
            Ident.TranslateAssign(trans);
        }
    }
}
