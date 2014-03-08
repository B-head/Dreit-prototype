using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class DeclateVariant : Scope
    {
        public IdentifierAccess Ident { get; set; }
        public Element ExplicitVariantType { get; set; }
        public Scope _DataType { get; set; }

        internal override Scope DataType
        {
            get
            {
                if(_DataType == null)
                {
                    throw new InvalidOperationException();
                }
                return _DataType; 
            }
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
            return Ident == null ? Name : Ident.Value;
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if (ExplicitVariantType != null)
            {
                _DataType = ExplicitVariantType.DataType;
            }
        }
    }
}
