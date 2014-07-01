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
        protected List<IScope> _Attribute;
        protected List<IDataType> _ArgumentType;
        protected IDataType _ReturnType;

        public override IReadOnlyList<IScope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<IDataType> ArgumentType
        {
            get { return _ArgumentType; }
        }

        public override IDataType ReturnType
        {
            get { return _ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
        }
    }
}
