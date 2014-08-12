using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class BooleanSymbol : VariantSymbol
    {
        public bool Value { get; private set; }

        public BooleanSymbol(bool value)
            : base(VariantType.Const)
        {
            Value = value;
            if(value)
            {
                Name = "true";
            }
            else
            {
                Name = "false";
            }
            _Attribute = new List<Scope>();
        }

        public override Scope ReturnType
        {
            get
            {
                if (_DataType != null)
                {
                    return _DataType;
                }
                _DataType = CurrentScope.NameResolution("Boolean").FindDataType();
                return _DataType;
            }
        }
    }
}
