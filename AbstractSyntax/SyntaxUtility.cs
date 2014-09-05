/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public static class SyntaxUtility
    {
        public static string ToNames(this IReadOnlyList<Scope> scopes)
        {
            var build = new StringBuilder();
            for(var i = 0; i < scopes.Count; ++i)
            {
                if(i > 0)
                {
                    build.Append(", ");
                }
                build.Append(scopes[i].Name);
            }
            return build.ToString();
        }

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
    }
}
