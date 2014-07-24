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

        internal override OverLoadReference NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope == null ? Root.UndefinedOverLord : CurrentScope.NameResolution(name);
            var s = TraversalChild(name, this).ToList();
            if (s.Count > 0)
            {
                n = new OverLoadReference(Root, n, s);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        private static IEnumerable<OverLoadSet> TraversalChild(string name, Element element)
        {
            var scope = element as Scope;
            if (scope != null && scope.ChildSymbols.ContainsKey(name))
            {
                yield return scope.ChildSymbols[name];
            }
            if(!(element is NameSpaceSymbol))
            {
                yield break;
            }
            foreach(var v in element)
            {
                foreach(var e in TraversalChild(name, v))
                {
                    yield return e;
                }
            }
        }
    }
}
