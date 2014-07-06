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
        public IReadOnlyList<AttributeSymbol> Qualify { get; private set; }

        public TypeQualifySymbol(Scope baseType, IReadOnlyList<AttributeSymbol> qualify)
        {
            BaseType = baseType;
            Qualify = qualify;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public static bool HasContainQualify(Scope type, AttributeSymbol qualify)
        {
            var t = type as TypeQualifySymbol;
            if(t == null)
            {
                return false;
            }
            return t.Qualify.Any(v => v == qualify);
        }
    }
}
