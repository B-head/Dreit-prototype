using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax
{
    class ScopeManager
    {
        private Dictionary<string, List<Scope>> ScopeSymbol;

        public ScopeManager()
        {
            ScopeSymbol = new Dictionary<string, List<Scope>>();
        }

        public void Append(Scope scope)
        {
            List<Scope> list;
            if(ScopeSymbol.ContainsKey(scope.Name))
            {
                list = ScopeSymbol[scope.Name];
            }
            else
            {
                list = new List<Scope>();
                ScopeSymbol[scope.Name] = list;
            }
            list.Add(scope);//仮
        }

        public void Merge(ScopeManager other)
        {
            foreach (var v in other.ScopeSymbol)
            {
                List<Scope> list;
                if (ScopeSymbol.ContainsKey(v.Key))
                {
                    list = ScopeSymbol[v.Key];
                }
                else
                {
                    list = new List<Scope>();
                    ScopeSymbol[v.Key] = list;
                }
                list.AddRange(v.Value);
            }
        }

        public Scope MatchScope(string name, List<Scope> type)
        {
            List<Scope> list;
            if(!ScopeSymbol.TryGetValue(name, out list))
            {
                return null;
            }
            var scope = list.Find(s => s.TypeMatch(type) == TypeMatchResult.PerfectMatch);
            return scope;
        }
    }
}
