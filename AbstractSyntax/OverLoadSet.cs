using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadSet
    {
        private Scope CurrentScope;
        private List<Scope> Symbols;
        private bool IsHoldAlias;

        internal OverLoadSet(Scope current)
        {
            CurrentScope = current;
            Symbols = new List<Scope>();
        }

        internal void Append(Scope scope)
        {
            if(scope is AliasDeclaration)
            {
                IsHoldAlias = true;
            }
            Symbols.Add(scope);
        }

        internal IEnumerable<Scope> TraversalDataType()
        {
            if(IsHoldAlias)
            {
                SpreadAlias();
            }
            foreach(var v in Symbols)
            {
                if(v.IsDataType)
                {
                    yield return v;
                }
            }
        }

        internal IEnumerable<TypeMatch> TraversalCall(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            if (IsHoldAlias)
            {
                SpreadAlias();
            }
            foreach (var s in Symbols)
            {
                foreach (var m in s.GetTypeMatch(pars, args))
                {
                    yield return m;
                }
            }
        }

        private void SpreadAlias()
        {
            var alias = Symbols.FindAll(v => v is AliasDeclaration);
            Symbols.RemoveAll(v => v is AliasDeclaration);
            foreach(var v in alias)
            {
                var ol = v.OverLoad;
                foreach (var t in ol.TraversalSets(true, true))
                {
                    foreach (var s in t.Symbols)
                    {
                        Append(s);
                    }
                }
            }
        }
    }
}
