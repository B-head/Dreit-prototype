using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class OverLoadScope : Scope
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

        public Scope TypeSelect()
        {
            return TypeSelect(new List<Scope>());
        }

        public Scope TypeSelect(List<Scope> type)
        {
            return OverLoad.Find(s => s.TypeMatch(type) == TypeMatchResult.PerfectMatch);
        }
    }
}
