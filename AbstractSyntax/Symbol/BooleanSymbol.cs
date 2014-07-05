using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class BooleanSymbol : Scope
    {
        public bool Value { get; private set; }
        private Scope _DataType;

        public BooleanSymbol(bool value)
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

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { ReturnType });
        }
    }
}
