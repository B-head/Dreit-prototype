using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class DeclareVariant : Scope
    {
        public Identifier Ident { get; set; }
        public Element ExplicitDataType { get; set; }

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
                case 1: return ExplicitDataType;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override string CreateName()
        {
            return Ident == null ? null : Ident.Value;
        }

        internal override void CheckDataType(Scope scope)
        {
            if (ExplicitDataType != null)
            {
                //DataType = NameResolution(ExplicitDataType.Value);
                if (DataType != null)
                {
                    //Trans.SetBaseType(DataType.FullPath);
                }
            }
            base.CheckDataType(scope);
        }

        internal override void CheckDataTypeAssign(Scope type)
        {
            if (DataType == null && type != null)
            {
                DataType = type;
                //Trans.SetBaseType(DataType.FullPath);
            }
            base.CheckDataTypeAssign(type);
        }

        internal override void Translate()
        {
            //Parent.Trans.GenelateLoad(FullPath);
        }

        internal override void TranslateAssign()
        {
            //Parent.Trans.GenelateStore(FullPath);
        }
    }
}
