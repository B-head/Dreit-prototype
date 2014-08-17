using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public abstract class TypeSymbol : Scope
    {
        private IReadOnlyList<GenericSymbol> _TacitGeneric;

        protected TypeSymbol()
        {

        }

        protected TypeSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public override TypeSymbol ReturnType
        {
            get { return Root.ClassManager.Issue(Root.Typeof, new TypeSymbol[] { this }, new TypeSymbol[0]); }
        }

        public override OverLoad OverLoad
        {
            get { return Root.SimplexManager.Issue(this); }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { throw new NotImplementedException(); }
        }

        public virtual IReadOnlyList<TypeSymbol> Inherit
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyList<GenericSymbol> TacitGeneric
        {
            get
            {
                if (_TacitGeneric != null)
                {
                    return _TacitGeneric;
                }
                var list = new List<GenericSymbol>();
                CurrentScope.BuildTacitGeneric(list);
                _TacitGeneric = list;
                return _TacitGeneric;
            }
        }

        internal override void BuildTacitGeneric(List<GenericSymbol> list)
        {
            if (CurrentScope != null)
            {
                CurrentScope.BuildTacitGeneric(list);
            }
            list.AddRange(Generics);
        }

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal virtual IEnumerable<OverLoadMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal virtual IEnumerable<TypeSymbol> EnumSubType()
        {
            yield break;
        }

        internal static bool HasAnyErrorType(params TypeSymbol[] scope)
        {
            return HasAnyErrorType((IReadOnlyList<TypeSymbol>)scope);
        }

        internal static bool HasAnyErrorType(IReadOnlyList<TypeSymbol> scope)
        {
            foreach (var v in scope)
            {
                if (v is VoidSymbol || v is UnknownSymbol || v is ErrorTypeSymbol)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
