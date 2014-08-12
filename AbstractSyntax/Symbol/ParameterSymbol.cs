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

        protected ParameterSymbol(TextPosition tp, VariantType type, Element def)
            : base(tp, type)
        {
            DefaultValue = def;
            AppendChild(DefaultValue);
        }

        public ParameterSymbol(string name, VariantType type, IReadOnlyList<Scope> attr, Scope dt)
            : base(name, type, attr, dt)
        {

        }

        public bool IsLoopParameter
        {
            get { return CurrentScope is LoopStatement || CurrentScope is ForStatement; }
        }
    }
}
