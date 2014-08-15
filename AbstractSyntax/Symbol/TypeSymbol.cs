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

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal virtual IEnumerable<OverLoadMatch> GetInstanceTypeMatch(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
        }

        internal virtual IEnumerable<TypeSymbol> EnumSubType()
        {
            yield break;
        }
    }
}
