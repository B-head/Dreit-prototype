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
        protected TypeSymbol()
        {

        }

        protected TypeSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public override TypeSymbol ReturnType
        {
            get { return Root.ClassManager.Issue(Root.Typeof, new TypeSymbol[] { this }); }
        }

        public override OverLoad OverLoad
        {
            get { return Root.SimplexManager.Issue(this); }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { throw new NotImplementedException(); }
        }

        public virtual IReadOnlyList<TypeSymbol> Inherit
        {
            get { throw new NotImplementedException(); }
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
    }
}
