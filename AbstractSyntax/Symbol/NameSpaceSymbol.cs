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
            if (SyntaxUtility.HasAnyAttribute(scope.Attribute, AttributeType.GlobalScope))
            {
                return true;
            }
            return false;
        }
    }
}
