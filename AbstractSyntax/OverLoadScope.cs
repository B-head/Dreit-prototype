using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadScope
    {
        private List<Scope> OverLoad;
        private VoidSymbol Void;
        public bool IsVoid { get { return Void != null; } }

        public OverLoadScope(VoidSymbol _void = null)
        {
            OverLoad = new List<Scope>();
            Void = _void;
        }

        public void Append(Scope scope)
        {
            if(IsVoid)
            {
                throw new InvalidOperationException();
            }
            OverLoad.Add(scope);
        }

        public void Merge(OverLoadScope other)
        {
            if (IsVoid)
            {
                throw new InvalidOperationException();
            }
            OverLoad.AddRange(other.OverLoad);
        }

        public DataType GetDataType()
        {
            if (IsVoid)
            {
                return Void;
            }
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
            if (IsVoid)
            {
                return Void;
            }
            return TypeSelect(new List<DataType>());
        }

        public Scope TypeSelect(List<DataType> type)
        {
            if (IsVoid)
            {
                return Void;
            }
            return OverLoad.Find(s => s.TypeMatch(type) == TypeMatchResult.PerfectMatch);
        }
    }
}
