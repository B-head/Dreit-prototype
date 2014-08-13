﻿using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class PropertySymbol : RoutineSymbol
    {
        public VariantSymbol Variant { get; private set; }
        public bool IsSet { get; private set; }

        public PropertySymbol(VariantSymbol variant, bool isSet)
            : base(RoutineType.Routine, TokenType.Unknoun)
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
            _Arguments = null;
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

        public override IReadOnlyList<ParameterSymbol> Arguments
        {
            get
            {
                if (_Arguments == null)
                {
                    if (IsSet)
                    {
                        _Arguments = SyntaxUtility.MakeParameters(Variant.CallReturnType);
                    }
                    else
                    {
                        _Arguments = new ParameterSymbol[] { };
                    }
                }
                return _Arguments;
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
