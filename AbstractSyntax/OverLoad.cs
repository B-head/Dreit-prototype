using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public abstract class OverLoad
    {
        public VariantSymbol FindVariant()
        {
            foreach (var v in TraversalVariant())
            {
                return v;
            }
            return Root.ErrorVariant;
        }

        public AttributeSymbol FindAttribute()
        {
            foreach (var v in TraversalAttribute())
            {
                return v;
            }
            throw new InvalidOperationException();
        }

        public OverLoadTypeMatch FindDataType()
        {
            var pars = new List<TypeSymbol>();
            foreach (var v in TraversalDataType(pars))
            {
                return v; //todo 型の選択をする。
            }
            return OverLoadTypeMatch.MakeNotType(Root.ErrorType);
        }

        public OverLoadCallMatch CallSelect()
        {
            return CallSelect(new List<TypeSymbol>());
        }

        public OverLoadCallMatch CallSelect(IReadOnlyList<TypeSymbol> args)
        {
            if (TypeSymbol.HasAnyErrorType(args))
            {
                return OverLoadCallMatch.MakeUnknown(Root.ErrorRoutine);
            }
            var result = OverLoadCallMatch.MakeNotCallable(Root.ErrorRoutine);
            var pars = new List<TypeSymbol>();
            foreach (var m in TraversalCall(pars, args))
            {
                var a = OverLoadCallMatch.GetMatchPriority(result.Result);
                var b = OverLoadCallMatch.GetMatchPriority(m.Result);
                if (a < b)
                {
                    result = m; //todo 優先順位が重複した場合の対処が必要。
                }
            }
            return result;
        }

        public abstract bool IsUndefined { get; }
        internal abstract Root Root { get; }
        internal abstract IEnumerable<Scope> TraversalChilds();
        internal abstract IEnumerable<VariantSymbol> TraversalVariant();
        internal abstract IEnumerable<AttributeSymbol> TraversalAttribute();
        internal abstract IEnumerable<OverLoadTypeMatch> TraversalDataType(IReadOnlyList<TypeSymbol> pars);
        internal abstract IEnumerable<OverLoadCallMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args);
    }
}
