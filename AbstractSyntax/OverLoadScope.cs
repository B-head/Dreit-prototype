using System;
using System.Collections.Generic;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadScope
    {
        private List<Scope> OverLoad;

        public OverLoadScope()
        {
            OverLoad = new List<Scope>();
        }

        public void Append(Scope scope)
        {
            OverLoad.Add(scope);
        }

        public void Merge(OverLoadScope other)
        {
            OverLoad.AddRange(other.OverLoad);
        }

        public DataType GetDataType()
        {
            var find = (DataType)OverLoad.Find(s => s is DataType);
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

        public Scope TypeSelect()
        {
            return TypeSelect(new List<DataType>());
        }

        public Scope TypeSelect(List<DataType> type)
        {
            return OverLoad.Find(s => s.TypeMatch(type) == TypeMatchResult.PerfectMatch);
        }
    }
}
