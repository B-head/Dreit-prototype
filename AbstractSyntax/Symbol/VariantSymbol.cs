using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class VariantSymbol : Scope
    {
        protected DataType _DataType;

        public override DataType DataType
        {
            get { return _DataType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new DataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new DataType[] { DataType });
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (DataType is UnknownSymbol)
            {
                CompileError("require-type");
            }
        }
    }
}
