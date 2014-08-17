using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ParameterSymbol : VariantSymbol
    {
        public Element DefaultValue { get; private set; }

        public ParameterSymbol()
        {

        }

        protected ParameterSymbol(TextPosition tp, VariantType type, Element def)
            : base(tp, type)
        {
            DefaultValue = def;
            AppendChild(DefaultValue);
        }

        public new void Initialize(string name, VariantType type, IReadOnlyList<AttributeSymbol> attr, TypeSymbol dt)
        {
            base.Initialize(name, type, attr, dt);
        }

        public bool IsLoopParameter
        {
            get { return CurrentScope is LoopStatement || CurrentScope is ForStatement; }
        }

        public static IReadOnlyList<ParameterSymbol> MakeParameters(params TypeSymbol[] types)
        {
            var ret = new List<ParameterSymbol>();
            for (var i = 0; i < types.Length; ++i)
            {
                var p = new ParameterSymbol();
                p.Initialize("@@arg" + (i + 1), VariantType.Let, new List<AttributeSymbol>(), types[i]);
                ret.Add(p);
            }
            return ret;
        }
    }
}
