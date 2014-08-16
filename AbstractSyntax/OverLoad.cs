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
        public virtual VariantSymbol FindVariant()
        {
            return FindVariant(false, false);
        }

        public VariantSymbol FindVariant(bool byMember, bool byStatic)
        {
            foreach (var v in TraversalVariant(byMember, byStatic))
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

        public virtual TypeSymbol FindDataType()
        {
            return FindDataType(new List<GenericsInstance>(), new List<TypeSymbol>(), false, false);
        }

        public TypeSymbol FindDataType(IReadOnlyList<GenericsInstance> inst,
            IReadOnlyList<TypeSymbol> pars, bool byMember, bool byStatic)
        {
            foreach (var v in TraversalDataType(inst, pars, byMember, byStatic))
            {
                return v; //todo 型の選択とインスタンス化をする。
            }
            return Root.ErrorType;
        }

        public OverLoadMatch CallSelect()
        {
            return CallSelect(new List<TypeSymbol>());
        }

        public virtual OverLoadMatch CallSelect(IReadOnlyList<TypeSymbol> args)
        {
            return CallSelect(new List<GenericsInstance>(), new List<TypeSymbol>(), args, false, false);
        }

        public OverLoadMatch CallSelect(IReadOnlyList<GenericsInstance> inst, 
            IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args, bool byMember, bool byStatic)
        {
            if (SyntaxUtility.HasAnyErrorType(pars) || SyntaxUtility.HasAnyErrorType(args))
            {
                return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
            }
            OverLoadMatch result = OverLoadMatch.MakeNotCallable(Root.ErrorRoutine);
            foreach (var m in TraversalCall(inst, pars, args, byMember, byStatic))
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
        internal abstract IEnumerable<VariantSymbol> TraversalVariant(bool byMember, bool byStatic);
        internal abstract IEnumerable<AttributeSymbol> TraversalAttribute();
        internal abstract IEnumerable<TypeSymbol> TraversalDataType(IReadOnlyList<GenericsInstance> inst,
            IReadOnlyList<TypeSymbol> pars, bool byMember, bool byStatic);
        internal abstract IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<GenericsInstance> inst,
            IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args, bool byMember, bool byStatic);
    }
}
