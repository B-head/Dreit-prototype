using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class UnknownOverLoad : OverLoad
    {
        public UnknownOverLoad(UnknownSymbol unknown)
            :base(unknown)
        {
        }

        public override void Append(Scope scope)
        {
            base.Append(scope);
        }

        public override void Merge(OverLoad other)
        {
            base.Merge(other);
        }

        public override DataType GetDataType()
        {
            return Unknown;
        }

        public override TypeMatch TypeSelect()
        {
            return TypeMatch.MakeNotCallable(Unknown);
        }

        public override TypeMatch TypeSelect(IReadOnlyList<DataType> type)
        {
            return TypeMatch.MakeNotCallable(Unknown);
        }
    }
}
