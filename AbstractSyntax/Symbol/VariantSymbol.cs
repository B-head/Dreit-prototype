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
        protected IDataType _DataType;

        public override IDataType DataType
        {
            get { return _DataType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { DataType });
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
