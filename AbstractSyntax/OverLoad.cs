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

        public virtual IDataType GetDataType()
        {
            if(isHoldAlias)
            {
                SpreadAlias();
            }
            var find = (IDataType)ScopeList.Find(s => s is IDataType);
            if(find != null)
            {
                return find;
            }
            var refer = TypeSelect();//todo 引数なしのDataTypeを返しているのを見直す。
            if (refer.Result == TypeMatchResult.NotCallable)
            {
                return Unknown;
            }
            return refer.Call.DataType; 
        }

        public virtual IScope SelectPlain()
        {
            if (isHoldAlias)
            {
                SpreadAlias();
            }
            var refer = TypeSelect();
            return refer.Call;
        }

        public virtual TypeMatch TypeSelect()
        {
            return TypeSelect(new List<IDataType>());
        }

        public virtual TypeMatch TypeSelect(IReadOnlyList<IDataType> type)
        {
            if (isHoldAlias)
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
                        result = b;
                    }
                    else if(result.Call == null)
                    {
                        result = b;
                    }
                }
            }
            return result;
        }

        private int GetMatchPriority(TypeMatchResult r)
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
