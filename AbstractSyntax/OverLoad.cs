using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoad : IReadOnlyList<Scope>
    {
        private List<Scope> ScopeList;
        private bool isHoldAlias;
        protected UnknownSymbol Unknown;

        public OverLoad(UnknownSymbol unknown)
        {
            ScopeList = new List<Scope>();
            Unknown = unknown;
        }

        public virtual void Append(Scope scope)
        {
            if(scope is AliasDirective)
            {
                isHoldAlias = true;
            }
            ScopeList.Add(scope);
        }

        public virtual void Merge(OverLoad other)
        {
            foreach(var v in other.ScopeList)
            {
                Append(v);
            }
        }

        public virtual DataType GetDataType()
        {
            if(isHoldAlias)
            {
                SpreadAlias();
            }
            var find = (DataType)ScopeList.Find(s => s is DataType);
            if(find != null)
            {
                return find;
            }
            var refer = TypeSelect();
            if (refer == null)
            {
                return Unknown;
            }
            return refer.DataType;
        }

        public virtual Scope TypeSelect()
        {
            if (isHoldAlias)
            {
                SpreadAlias();
            }
            return TypeSelect(new List<DataType>());
        }

        public virtual Scope TypeSelect(IReadOnlyList<DataType> type)
        {
            if (isHoldAlias)
            {
                SpreadAlias();
            }
            return ScopeList.Find(s => s.TypeMatch(type) == TypeMatchResult.PerfectMatch);
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
