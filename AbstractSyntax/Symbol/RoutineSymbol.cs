using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineSymbol : Scope
    {
        public TokenType Operator { get; set; }
        protected List<DataType> _ArgumentType;
        protected DataType _ReturnType;

        public virtual List<DataType> ArgumentType
        {
            get { return _ArgumentType; }
        }

        public virtual DataType ReturnType
        {
            get { return _ReturnType; }
        }

        public override DataType DataType
        {
            get { return ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
        }
    }
}
