using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class TypeQualifySymbol : Scope
    {
        public Scope BaseType { get; private set; }
        public IReadOnlyList<Scope> Qualify { get; private set; }

        public TypeQualifySymbol(Scope baseType, IReadOnlyList<Scope> qualify)
        {
            BaseType = baseType;
            Qualify = qualify;
        }

        public override bool IsDataType
        {
            get { return true; }
        }
    }
}
