using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class GenericSymbol : Scope
    {
        protected IReadOnlyList<Scope> _Attribute;
        protected IReadOnlyList<Scope> _Constraint;

        protected GenericSymbol(TextPosition tp, string name)
            :base(tp)
        {
            Name = name;
        }

        public GenericSymbol(string name, IReadOnlyList<Scope> attr, IReadOnlyList<Scope> constraint)
        {
            Name = name;
            _Attribute = attr;
            _Constraint = constraint;
        }

        public override bool IsDataType
        {
            get { return true; }
        }
    }
}
