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
        public Identifier ExplicitDataType { get; set; }
        public Scope DataType { get; set; }

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

        protected override string ElementInfo()
        {
            StringBuilder builder = new StringBuilder(base.ElementInfo());
            if(Ident != null)
            {
                builder.Append(Ident.Value);
            }
            if(ExplicitDataType != null)
            {
                builder.Append(ExplicitDataType.Value);
            }
            if(DataType != null)
            {
                builder.Append(" (" + DataType.Name + ")");
            }
            else
            {
                builder.Append(" (<null>)");
            }
            return builder.ToString();
        }

        internal override Translator CreateTranslator(Translator trans)
        {
            return trans.CreateVariant(FullPath);
        }

        internal override void CheckDataType()
        {
            if (ExplicitDataType != null)
            {
                DataType = NameResolution(ExplicitDataType.Value);
                if (DataType != null)
                {
                    Trans.SetBaseType(DataType.FullPath);
                }
            }
            base.CheckDataType();
        }

        internal override void CheckDataTypeAssign(Scope type)
        {
            if (DataType == null && type != null)
            {
                DataType = type;
                Trans.SetBaseType(DataType.FullPath);
            }
            base.CheckDataTypeAssign(type);
        }

        internal override Scope GetDataType()
        {
            return DataType;
        }

        internal override void Translate()
        {
            Parent.Trans.GenelateLoad(FullPath);
        }

        internal override void TranslateAssign()
        {
            Parent.Trans.GenelateStore(FullPath);
        }
    }
}
