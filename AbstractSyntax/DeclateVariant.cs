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
        public bool IsImport { get; set; }
        public Identifier Ident { get; set; }
        public Element ExplicitVariantType { get; set; }

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

        internal override void CheckDataType(Scope scope)
        {
            if (ExplicitVariantType != null)
            {
                DataType = ExplicitVariantType.DataType;
            }
            base.CheckDataType(scope);
        }

        internal override void CheckDataTypeAssign(Scope type)
        {
            if (DataType == null && type != null)
            {
                DataType = type;
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
