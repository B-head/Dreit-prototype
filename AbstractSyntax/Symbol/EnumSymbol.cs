using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class EnumSymbol : TypeSymbol
    {
        public ProgramContext Block { get; private set; }
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected Scope _BaseType;

        protected EnumSymbol(TextPosition tp, string name, ProgramContext block)
            :base(tp)
        {
            Name = name;
            Block = block;
            AppendChild(Block);
        }

        public EnumSymbol(string name, ProgramContext block, IReadOnlyList<AttributeSymbol> attr, Scope bt)
        {
            Name = name;
            Block = block;
            _Attribute = attr;
            _BaseType = bt;
            AppendChild(Block);
        }
    }
}
