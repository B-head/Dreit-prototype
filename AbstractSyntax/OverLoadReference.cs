using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    delegate IReadOnlyList<OverLoadReference> InitInherits();

    [Serializable]
    public class OverLoadReference
    {
        private Root Root;
        public OverLoadReference Next { get; private set; }
        private IReadOnlyList<OverLoadReference> _Inherits;
        private InitInherits InitInherits;
        public IReadOnlyList<OverLoadSet> Sets { get; private set; }

        internal OverLoadReference(Root root, OverLoadReference next, params OverLoadSet[] sets)
        {
            Root = root;
            Next = next;
            InitInherits = () => new List<OverLoadReference>();
            Sets = sets;
        }

        internal OverLoadReference(Root root, OverLoadReference next, IReadOnlyList<OverLoadSet> sets)
        {
            Root = root;
            Next = next;
            InitInherits = () => new List<OverLoadReference>();
            Sets = sets;
        }

        internal OverLoadReference(Root root, OverLoadReference next, InitInherits initInherits, params OverLoadSet[] sets)
        {
            Root = root;
            Next = next;
            InitInherits = initInherits;
            Sets = sets;
        }

        public bool IsUndefined
        {
            get { return Next == null; }
        }

        public IReadOnlyList<OverLoadReference> Inherits
        {
            get
            {
                if (_Inherits == null)
                {
                    _Inherits = InitInherits();
                }
                return _Inherits;
            }
        }

        public Scope FindDataType()
        {
            foreach(var s in TraversalSets(true, false))
            {
                foreach(var v in s.TraversalDataType())
                {
                    return v;
                }
            }
            return Root.Unknown;
        }

        public TypeMatch CallSelect()
        {
            return CallSelect(new List<Scope>());
        }

        public TypeMatch CallSelect(IReadOnlyList<Scope> type)
        {
            TypeMatch result = TypeMatch.MakeNotCallable(Root.Unknown);
            foreach (var s in TraversalSets(true, true))
            {
                foreach (var m in s.TraversalCall(type))
                {
                    if (GetMatchPriority(result.Result) < GetMatchPriority(m.Result))
                    {
                        result = m; //todo 優先順位が重複した場合の対処が必要。
                    }
                }
            }
            return result;
        }

        internal IEnumerable<OverLoadSet> TraversalSets(bool byNext, bool byInherit)
        {
            foreach(var s in Sets)
            {
                yield return s;
            }
            if (byInherit)
            {
                foreach (var i in Inherits)
                {
                    foreach (var s in i.TraversalSets(false, true))
                    {
                        yield return s;
                    }
                }
            }
            if (byNext && Next != null)
            {
                foreach (var s in Next.TraversalSets(true, byInherit))
                {
                    yield return s;
                }
            }
        }

        private static int GetMatchPriority(TypeMatchResult r)
        {
            switch (r)
            {
                case TypeMatchResult.Unknown: return 10;
                case TypeMatchResult.PerfectMatch: return 9;
                case TypeMatchResult.ConvertMatch: return 8;
                case TypeMatchResult.UnmatchType: return 2;
                case TypeMatchResult.UnmatchCount: return 1;
                case TypeMatchResult.NotCallable: return 0;
                default: throw new ArgumentException("r");
            }
        }
    }
}
