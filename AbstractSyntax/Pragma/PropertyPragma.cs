using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class PropertyPragma : RoutineSymbol
    {
        public VariantSymbol Variant { get; private set; }
        public bool IsSet { get; private set; }

        public PropertyPragma(VariantSymbol variant, bool isSet)
        {
            if(isSet)
            {
                Name = "@@set";
            }
            else
            {
                Name = "@@get";
            }
            Variant = variant;
            IsSet = isSet;
            _Attribute = null;
            _ArgumentTypes = null;
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get
            {
                if (_Attribute == null)
                {
                    _Attribute = Variant.Attribute;
                }
                return _Attribute;
            }
        }

        public override IReadOnlyList<Scope> ArgumentTypes
        {
            get
            {
                if (_ArgumentTypes == null)
                {
                    if (IsSet)
                    {
                        _ArgumentTypes = new Scope[] { Variant.CallReturnType };
                    }
                    else
                    {
                        _ArgumentTypes = new Scope[] { };
                    }
                }
                return _ArgumentTypes;
            }
        }

        public override Scope CallReturnType
        {
            get
            {
                if (_CallReturnType == null)
                {
                    _CallReturnType = Variant.CallReturnType;
                }
                return _CallReturnType;
            }
        }
    }
}
