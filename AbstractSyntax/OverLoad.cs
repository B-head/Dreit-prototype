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
            return null;
        }

        public TypeSymbol FindDataType()
        {
            foreach (var v in TraversalDataType())
            {
                return v;
            }
            return Root.ErrorType;
        }

        public OverLoadMatch CallSelect()
        {
            return CallSelect(new List<TypeSymbol>(), new List<TypeSymbol>());
        }

        public OverLoadMatch CallSelect(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            if (SyntaxUtility.HasAnyErrorType(pars) || SyntaxUtility.HasAnyErrorType(args))
            {
                return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
            }
            OverLoadMatch result = OverLoadMatch.MakeNotCallable(Root.ErrorRoutine);
            foreach (var m in TraversalCall(pars, args))
            {
                var a = OverLoadMatch.GetMatchPriority(result.Result);
                var b = OverLoadMatch.GetMatchPriority(m.Result);
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
        internal abstract IEnumerable<TypeSymbol> TraversalDataType();
        internal abstract IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args);
    }
}
