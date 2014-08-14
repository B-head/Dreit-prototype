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
        public Scope FindDataType()
        {
            foreach (var v in TraversalDataType())
            {
                return v;
            }
            return Root.ErrorType;
        }

        public OverLoadMatch CallSelect()
        {
            return CallSelect(new List<Scope>(), new List<Scope>());
        }

        public OverLoadMatch CallSelect(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            if (OverLoadMatch.CheckErrorType(pars) || OverLoadMatch.CheckErrorType(args))
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
        internal abstract IEnumerable<Scope> TraversalDataType();
        internal abstract IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args);
    }
}
