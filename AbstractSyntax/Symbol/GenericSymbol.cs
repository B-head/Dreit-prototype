﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class GenericSymbol : TypeSymbol
    {
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected IReadOnlyList<Scope> _Constraint;

        protected GenericSymbol(TextPosition tp, string name)
            :base(tp)
        {
            Name = name;
        }

        public GenericSymbol(string name, IReadOnlyList<AttributeSymbol> attr, IReadOnlyList<Scope> constraint)
        {
            Name = name;
            _Attribute = attr;
            _Constraint = constraint;
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
        }
    }
}
