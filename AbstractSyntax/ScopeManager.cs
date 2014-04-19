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

        public bool Append(Scope scope)
        {
            if(ScopeSymbol.ContainsKey(scope.Name))
            {
                return false;
            }
            var list = ScopeSymbol[scope.Name];
            if(list == null)
            {
                list = new List<Scope>();
                ScopeSymbol[scope.Name] = list;
            }
            list.Add(scope);//仮
            return true;
        }

        public Scope MatchScope(string name)
        {
            return MatchScope(name, new List<Scope>());
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

        /*public bool CheckType()
        {
            Type temp = null;
            for (var i = 0; i < _OverLoad.Count; i++)
            {
                if (i == 0)
                {
                    temp = _OverLoad[i].GetType();
                    continue;
                }
                if (!(_OverLoad[i] is DeclateRoutine || _OverLoad[i] is DeclateClass))
                {
                    return false;
                }
                if (temp != _OverLoad[i].GetType())
                {
                    return false;
                }
            }
            return true;
        }*/
    }
}
