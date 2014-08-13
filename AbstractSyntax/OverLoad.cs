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
            return Root.Unknown;
        }

        public OverLoadMatch CallSelect()
        {
            return CallSelect(new List<Scope>(), new List<Scope>());
        }

        public OverLoadMatch CallSelect(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            OverLoadMatch result = OverLoadMatch.MakeNotCallable(Root.Unknown);
            foreach (var m in TraversalCall(pars, args))
            {
                var a = GetMatchPriority(result.Result);
                var b = GetMatchPriority(m.Result);
                if (a < b)
                {
                    result = m; //todo 優先順位が重複した場合の対処が必要。
                }
            }
            return result;
        }

        private static int GetMatchPriority(TypeMatchResult r)
        {
            switch (r)
            {
                case TypeMatchResult.Unknown: return 10;
                case TypeMatchResult.PerfectMatch: return 9;
                case TypeMatchResult.ConvertMatch: return 8;
                case TypeMatchResult.AmbiguityMatch: return 7;
                case TypeMatchResult.UnmatchArgumentType: return 4;
                case TypeMatchResult.UnmatchArgumentCount: return 3;
                case TypeMatchResult.UnmatchParameterType: return 2;
                case TypeMatchResult.UnmatchParameterCount: return 1;
                case TypeMatchResult.NotCallable: return 0;
                default: throw new ArgumentException("r");
            }
        }

        public abstract bool IsUndefined { get; }
        internal abstract Root Root { get; }
        internal abstract IEnumerable<Scope> TraversalChilds();
        internal abstract IEnumerable<Scope> TraversalDataType();
        internal abstract IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args);
    }
}
