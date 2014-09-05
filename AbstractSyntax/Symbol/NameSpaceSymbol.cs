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
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class NameSpaceSymbol : Scope
    {
        public NameSpaceSymbol()
        {

        }

        public NameSpaceSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public NameSpaceSymbol(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public NameSpaceSymbol(string name)
        {
            Name = name;
        }

        public void Append(Element element)
        {
            AppendChild(element);
        }

        //todo 同じオブジェクトが二重登録されるバグを取る。
        internal override OverLoadChain NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope == null ? Root.UndefinedOverLord : CurrentScope.NameResolution(name);
            var s = TraversalChild(name, this).ToArray();
            if (s.Length > 0)
            {
                n = new OverLoadChain(this, n, s);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        private static IEnumerable<OverLoadSet> TraversalChild(string name, Element element)
        {
            var scope = element as Scope;
            if (scope == null || !HasGlobalScope(scope))
            {
                yield break;
            }
            if (scope.ChildSymbols.ContainsKey(name))
            {
                yield return scope.ChildSymbols[name];
            }
            foreach(var v in element)
            {
                foreach(var e in TraversalChild(name, v))
                {
                    yield return e;
                }
            }
        }

        private static bool HasGlobalScope(Scope scope)
        {
            if(scope is NameSpaceSymbol)
            {
                return true;
            }
            if (scope.Attribute.HasAnyAttribute(AttributeType.GlobalScope))
            {
                return true;
            }
            return false;
        }
    }
}
