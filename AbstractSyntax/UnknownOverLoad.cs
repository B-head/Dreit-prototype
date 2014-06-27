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
            throw new NotSupportedException();
        }

        public override void Merge(OverLoad other)
        {
            throw new NotSupportedException();
        }

        public override IDataType GetDataType()
        {
            return Unknown;
        }

        public override IScope SelectPlain()
        {
            return Unknown;
        }

        public override TypeMatch TypeSelect()
        {
            return TypeMatch.MakeNotCallable(Unknown);
        }

        public override TypeMatch TypeSelect(IReadOnlyList<IDataType> type)
        {
            return TypeMatch.MakeNotCallable(Unknown);
        }
    }
}
