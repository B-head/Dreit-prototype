using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    //todo ジェネリッククラスを模したReferSymbol/TypeofSymbolに置き換える。
    [Serializable]
    public class QualifyTypeSymbol : Scope
    {
        public Scope BaseType { get; private set; }
        public IReadOnlyList<AttributeSymbol> Qualify { get; private set; }

        public QualifyTypeSymbol(Scope baseType, IReadOnlyList<AttributeSymbol> qualify)
        {
            BaseType = baseType;
            Qualify = qualify;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        internal override OverLoadChain NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = BaseType.NameResolution(name);
            if (ChildSymbols.ContainsKey(name))
            {
                var s = ChildSymbols[name];
                n = new OverLoadChain(this, n, s);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        public static bool HasContainQualify(Scope type, AttributeSymbol qualify)
        {
            var t = type as QualifyTypeSymbol;
            if(t == null)
            {
                return false;
            }
            return t.Qualify.Any(v => v == qualify);
        }
    }
}
