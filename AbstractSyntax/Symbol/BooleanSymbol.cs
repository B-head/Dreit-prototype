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
        private IDataType _DataType;

        public BooleanSymbol(bool value)
        {
            Value = value;
        }

        public override IDataType ReturnType
        {
            get
            {
                if (_DataType != null)
                {
                    return _DataType;
                }
                _DataType = CurrentScope.NameResolution("Boolean").GetDataType();
                return _DataType;
            }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { ReturnType });
        }
    }
}
