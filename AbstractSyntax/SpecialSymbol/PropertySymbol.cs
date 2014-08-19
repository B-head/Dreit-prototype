using AbstractSyntax.Symbol;
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
        public ClassSymbol Type { get; private set; }
        public bool IsSet { get; private set; }

        public PropertySymbol(string name, ClassSymbol type, bool isSet)
            : base(RoutineType.Routine, TokenType.Unknoun)
        {
            Name = name;
            Type = type;
            IsSet = isSet;
            _Attribute = null;
            _Arguments = null;
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute == null)
                {
                    _Attribute = Type.Attribute;
                }
                return _Attribute;
            }
        }

        public override IReadOnlyList<ArgumentSymbol> Arguments
        {
            get
            {
                if (_Arguments == null)
                {
                    if (IsSet)
                    {
                        _Arguments = ArgumentSymbol.MakeParameters(Type);
                    }
                    else
                    {
                        _Arguments = new ArgumentSymbol[] { };
                    }
                }
                return _Arguments;
            }
        }

        public override TypeSymbol CallReturnType
        {
            get { return Type; }
        }
    }
}
