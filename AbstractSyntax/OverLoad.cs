using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoad
    {
        private List<Scope> ScopeList;
        private bool isHoldAlias;

        public OverLoad()
        {
            ScopeList = new List<Scope>();
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
                throw new InvalidOperationException();
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

        public virtual Scope TypeSelect(List<DataType> type)
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
    }
}
