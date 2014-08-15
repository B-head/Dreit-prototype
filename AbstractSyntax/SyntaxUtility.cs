using AbstractSyntax.SpecialSymbol;
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

        public static IReadOnlyList<TypeSymbol> GetDataTypes(this IReadOnlyList<Element> list)
        {
            var result = new List<TypeSymbol>();
            foreach (var v in list)
            {
                result.Add(v.ReturnType);
            }
            return result;
        }

        internal static bool HasAnyAttribute(this IReadOnlyList<AttributeSymbol> attribute, params AttributeType[] type)
        {
            foreach (var v in attribute)
            {
                if (type.Any(t => t == v.AttributeType))
                {
                    return true;
                }
            }
            return false;
        }

        public static IReadOnlyList<ParameterSymbol> MakeParameters(params TypeSymbol[] types)
        {
            var ret = new List<ParameterSymbol>();
            for(var i = 0; i < types.Length; ++i)
            {
                var p = new ParameterSymbol("@@arg" + (i + 1), VariantType.Let, new List<AttributeSymbol>(), types[i]);
                ret.Add(p);
            }
            return ret;
        }

        public static bool HasVariadicArguments(this Scope scope)
        {
            var r = scope as RoutineSymbol;
            if(r == null)
            {
                return false;
            }
            if(r.Arguments.Count == 0)
            {
                return false;
            }
            return r.Arguments.Last().Attribute.HasAnyAttribute(AttributeType.Variadic);
        }

        internal static bool HasAnyErrorType(params TypeSymbol[] scope)
        {
            return HasAnyErrorType((IReadOnlyList<TypeSymbol>)scope);
        }

        internal static bool HasAnyErrorType(IReadOnlyList<TypeSymbol> scope)
        {
            foreach (var v in scope)
            {
                if (v is VoidSymbol || v is UnknownSymbol || v is ErrorTypeSymbol)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
