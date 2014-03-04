using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DeclareVariant : Scope
    {
        public Identifier Ident { get; set; }
        public Element ExplicitVariantType { get; set; }
        private Scope _DataType;

        internal override Scope DataType
        {
            get { return _DataType; }
        }

        public override bool IsReference
        {
            get { return true; }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element Child(int index)
        {
            switch (index)
            {
                case 0: return Ident;
                case 1: return ExplicitVariantType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void SpreadTranslate(Translator trans)
        {
            if (!IsImport)
            {
                trans.CreateVariant(FullPath, DataType.FullPath);
                base.SpreadTranslate(trans);
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if (ExplicitVariantType != null)
            {
                _DataType = ExplicitVariantType.DataType;
            }
        }

        internal override void CheckDataTypeAssign(Scope type)
        {
            if (_DataType == null && type != null)
            {
                _DataType = type;
            }
            base.CheckDataTypeAssign(type);
        }

        internal override void Translate(Translator trans)
        {
            trans.GenelateLoad(FullPath);
        }

        internal override void TranslateAssign(Translator trans)
        {
            trans.GenelateStore(FullPath);
        }
    }
}
