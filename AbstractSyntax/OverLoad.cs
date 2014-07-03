using AbstractSyntax.Directive;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoad : IReadOnlyList<IScope>
    {
        private List<Scope> ScopeList;
        private bool IsHoldAlias;
        private bool Freeze;
        private UnknownSymbol Unknown;

        public OverLoad(UnknownSymbol unknown, bool freeze = false)
        {
            ScopeList = new List<Scope>();
            Unknown = unknown;
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

        public IDataType FindDataType()
        {
            if(IsHoldAlias)
            {
                SpreadAlias();
            }
            var find = (IDataType)ScopeList.Find(s => s is IDataType);
            return find == null ? Unknown : find;
        }

        public TypeMatch CallSelect()
        {
            return CallSelect(new List<IDataType>());
        }

        public TypeMatch CallSelect(IReadOnlyList<IDataType> type) //todo アクセス可能性を考慮した検索に対応する。
        {
            if (IsHoldAlias)
            {
                SpreadAlias();
            }
            TypeMatch result = TypeMatch.MakeNotCallable(Unknown);
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
                case TypeMatchResult.ConvertMatch: return 5;
                default: return 0;
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

        public IScope this[int index]
        {
            get { return ScopeList[index]; }
        }

        public int Count
        {
            get { return ScopeList.Count; }
        }

        public IEnumerator<IScope> GetEnumerator()
        {
            return ScopeList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
