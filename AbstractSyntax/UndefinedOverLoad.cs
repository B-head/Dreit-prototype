using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class UndefinedOverLoad : OverLoad
    {
        private UndefinedSymbol Undefined;

        public UndefinedOverLoad(UndefinedSymbol undefined)
        {
            Undefined = undefined;
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
            return Undefined;
        }

        public override Scope TypeSelect()
        {
            return Undefined;
        }

        public override Scope TypeSelect(List<DataType> type)
        {
            return Undefined;
        }
    }
}
