using AbstractSyntax.Directive;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoad : IReadOnlyList<Scope>
    {
        private Root Root;
        private List<Scope> ScopeList;
        private bool IsHoldAlias;
        private bool Freeze;

        public OverLoad(Root root, bool freeze = false)
        {
            Root = root;
            ScopeList = new List<Scope>();
            Freeze = freeze;
        }

        public void Append(Scope scope)
        {
            if(Freeze)
            {
                throw new InvalidOperationException();
            }
            if(scope is AliasDirective)
            {
                IsHoldAlias = true;
            }
            ScopeList.Add(scope);
        }

        public void Merge(OverLoad other)
        {
            if (Freeze)
            {
                throw new InvalidOperationException();
            }
            if(other == null)
            {
                throw new ArgumentNullException();
            }
            foreach(var v in other.ScopeList)
            {
                Append(v);
            }
        }

        public bool IsUndefined
        {
            get { return ScopeList.Count == 0; }
        }

        public Scope FindDataType()
        {
            if(IsHoldAlias)
            {
                SpreadAlias();
            }
            var find = (Scope)ScopeList.Find(s => s.IsDataType);
            return find == null ? Root.Unknown : find;
        }

        public TypeMatch CallSelect()
        {
            return CallSelect(new List<Scope>());
        }

        public TypeMatch CallSelect(IReadOnlyList<Scope> type) //todo アクセス可能性を考慮した検索に対応する。
        {
            if (IsHoldAlias)
            {
                SpreadAlias();
            }
            TypeMatch result = TypeMatch.MakeNotCallable(Root.Unknown);
            foreach(var a in ScopeList)
            {
                foreach(var b in a.GetTypeMatch(type))
                {
                    if (GetMatchPriority(result.Result) < GetMatchPriority(b.Result))
                    {
                        result = b; //todo 優先順位が重複した場合の対処が必要。
                    }
                }
            }
            return result;
        }

        private static int GetMatchPriority(TypeMatchResult r)
        {
            switch(r)
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

        private void SpreadAlias()
        {
            var alias = ScopeList.FindAll(v => v is AliasDirective);
            ScopeList.RemoveAll(v => v is AliasDirective);
            foreach(var v in alias)
            {
                var ol = ((AliasDirective)v).RefarenceResolution();
                Merge(ol);
            }
        }

        public Scope this[int index]
        {
            get { return ScopeList[index]; }
        }

        public int Count
        {
            get { return ScopeList.Count; }
        }

        public IEnumerator<Scope> GetEnumerator()
        {
            return ScopeList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
