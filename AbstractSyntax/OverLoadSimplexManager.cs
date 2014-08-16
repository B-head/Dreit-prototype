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
        private Dictionary<Element, OverLoadSimplex> SimplexDictionary;

        public OverLoadSimplexManager()
        {
            SimplexDictionary = new Dictionary<Element, OverLoadSimplex>();
        }
        
        public OverLoadSimplex Issue(Element scope)
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
