using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ArgumentSymbol : VariantSymbol
    {
        public ArgumentSymbol()
        {

        }

        protected ArgumentSymbol(TextPosition tp, VariantType type, Element def)
            : base(tp, type, def)
        {
        }

        public new void Initialize(string name, VariantType type, IReadOnlyList<AttributeSymbol> attr, TypeSymbol dt, Element def)
        {
            base.Initialize(name, type, attr, dt, def);
        }

        internal static IReadOnlyList<ArgumentSymbol> MakeParameters(params TypeSymbol[] types)
        {
            var ret = new List<ArgumentSymbol>();
            for (var i = 0; i < types.Length; ++i)
            {
                var p = new ArgumentSymbol();
                p.Initialize("@@arg" + (i + 1), VariantType.Let, new List<AttributeSymbol>(), types[i]);
                ret.Add(p);
            }
            return ret;
        }

        internal static bool HasVariadic(IReadOnlyList<Scope> f)
        {
            if (f.Count == 0)
            {
                return false;
            }
            return f.Last().Attribute.HasAnyAttribute(AttributeType.Variadic);
        }
    }
}
