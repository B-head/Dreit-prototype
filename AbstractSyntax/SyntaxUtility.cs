using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public static class SyntaxUtility
    {
        public static IReadOnlyList<T> FindElements<T>(this IReadOnlyList<Element> list) where T : Element
        {
            var result = new List<T>();
            foreach (var v in list)
            {
                if (v is T)
                {
                    result.Add((T)v);
                }
            }
            return result;
        }

        public static IReadOnlyList<Scope> GetDataTypes(this IReadOnlyList<Element> list)
        {
            var result = new List<Scope>();
            foreach (var v in list)
            {
                result.Add(v.ReturnType);
            }
            return result;
        }

    }
}
