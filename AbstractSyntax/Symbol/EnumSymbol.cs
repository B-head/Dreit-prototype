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
        protected TypeSymbol _BaseType;

        protected EnumSymbol(TextPosition tp, string name, ProgramContext block)
            : base(tp)
        {
            Name = name;
            Block = block;
            AppendChild(Block);
        }

        public EnumSymbol(string name, ProgramContext block, IReadOnlyList<AttributeSymbol> attr, TypeSymbol bt)
        {
            Name = name;
            Block = block;
            _Attribute = attr;
            _BaseType = bt;
            AppendChild(Block);
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get { return new List<GenericSymbol>(); }
        }

        public override IReadOnlyList<TypeSymbol> Inherit
        {
            get { return new TypeSymbol[] { BaseType }; }
        }

        public virtual TypeSymbol BaseType
        {
            get { return _BaseType; }
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            return BaseType.GetTypeMatch(inst, pars, args);
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            return BaseType.GetInstanceMatch(inst, pars, args);
        }
    }
}
