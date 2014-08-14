using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadSimplexManager
    {
        private Dictionary<Scope, OverLoadSimplex> SimplexDictionary;

        public OverLoadSimplexManager()
        {
            SimplexDictionary = new Dictionary<Scope,OverLoadSimplex>();
        }
        
        public OverLoadSimplex Issue(Scope scope)
        {
            if(SimplexDictionary.ContainsKey(scope))
            {
                return SimplexDictionary[scope];
            }
            var ret = new OverLoadSimplex(scope);
            SimplexDictionary.Add(scope, ret);
            return ret;
        }
    }
}
