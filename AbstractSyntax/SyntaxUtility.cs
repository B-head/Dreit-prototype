using AbstractSyntax.Symbol;
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

        internal static bool HasAnyAttribute(IReadOnlyList<Scope> attribute, params AttributeType[] type)
        {
            foreach (var v in attribute)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                if (type.Any(t => t == a.AttributeType))
                {
                    return true;
                }
            }
            return false;
        }

        public static IReadOnlyList<ParameterSymbol> MakeParameters(params Scope[] types)
        {
            var ret = new List<ParameterSymbol>();
            for(var i = 0; i < types.Length; ++i)
            {
                var p = new ParameterSymbol("@@arg" + (i + 1), VariantType.Let, new List<Scope>(), types[i]);
                ret.Add(p);
            }
            return ret;
        }
    }
}
